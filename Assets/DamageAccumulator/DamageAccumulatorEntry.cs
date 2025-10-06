using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;
using UnityEngine.UI;

public class DamageAccumulatorEntry : MonoBehaviour
{
	[SerializeField]
	GameObject percussionIcon;

	[SerializeField]
	GameObject particlesPrefab;

	[SerializeField]
	GameObject bassIcon;

	[SerializeField]
	GameObject melodyIcon;

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

		var iconPrefab = Type switch
		{
			TuneType.Melody => melodyIcon,
			TuneType.Bass => bassIcon,
			TuneType.Percussion => percussionIcon,
			_ => null,
		};

		Debug.Assert(iconPrefab != null, "Invalid TuneType for damage accumulator entry");

		var icon = Instantiate(iconPrefab, transform);
		var iconRect = icon.GetComponent<RectTransform>();
		iconRect.anchorMin = Vector2.zero;
		iconRect.anchorMax = Vector2.one;
		iconRect.anchoredPosition = Vector2.zero;
		iconRect.sizeDelta = Vector2.zero;
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

	void OnDestroy()
	{
		if (particlesPrefab != null)
		{
			var go = Instantiate(particlesPrefab, transform.parent);
			go.transform.position = transform.position;
		}
	}
}