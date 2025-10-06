using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.U2D;

public class DamageAccumulatorEntry : MonoBehaviour
{
	[SerializeField]
	AnimationCurve curve;

	[SerializeField]
	float duration = 0.5f;

	[System.NonSerialized]
	public Vector2 TargetPosition;

	bool lerp = false;

	void Update()
	{
		if (!lerp)
			return;

		transform.position = Vector2.Lerp(
			transform.position,
			TargetPosition,
			Time.deltaTime * 10f
		);
	}

	void MoveToTarget()
	{
		IEnumerator Impl()
		{
			float startTime = Time.time;
			float endTime = startTime + duration;

			Vector2 startPosition = transform.position;

			while (Time.time < endTime)
			{
				float t = (Time.time - startTime) / duration;
				t = curve.Evaluate(t);
				transform.position = Vector2.LerpUnclamped(startPosition, TargetPosition, t);
				yield return null;
			}
			transform.position = TargetPosition;
			lerp = true;
		}

		StartCoroutine(Impl());
	}
}