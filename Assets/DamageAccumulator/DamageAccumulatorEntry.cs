using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;
using UnityEngine.UI;

public class DamageAccumulatorEntry : MonoBehaviour
{
	[SerializeField]
	Theme theme;

	[SerializeField]
	AnimationCurve curve;

	[SerializeField]
	float duration = 0.5f;

	[System.NonSerialized]
	public Vector2 TargetPosition;

	[SerializeField]
	float swayMagnitude = 5f;

	public UnityEvent ReachedTargetEvent = new();

	[System.NonSerialized]
	public TuneType Type;

	bool lerp = true;

	Vector2 sway = Vector2.zero;

	Vector2 ActualTargetPosition => TargetPosition + sway;

	float startTime = 0f;

	void Start()
	{
		startTime = Time.time;
		var image = GetComponentInChildren<Image>();
		image.color = theme.GetColor(Type);
	}

	void Update()
	{
		float t = Time.time - startTime;
		sway = new Vector2(
			math.sin(t * 3f + (int)Type * 10) * swayMagnitude,
			math.cos(t * 4f + (int)Type * 15) * swayMagnitude
		);

		if (!lerp)
			return;

		transform.position = Vector2.Lerp(
			transform.position,
			ActualTargetPosition,
			Time.deltaTime * 10f
		);
	}

	public void MoveToTarget()
	{
		lerp = false;
		IEnumerator Impl()
		{
			float startTime = Time.time;
			float endTime = startTime + duration;

			Vector2 startPosition = transform.position;

			while (Time.time < endTime)
			{
				float t = (Time.time - startTime) / duration;
				t = curve.Evaluate(t);
				transform.position = Vector2.LerpUnclamped(startPosition, ActualTargetPosition, t);
				yield return null;
			}
			ReachedTargetEvent.Invoke();
			transform.position = ActualTargetPosition;
			lerp = true;
		}

		StartCoroutine(Impl());
	}
}