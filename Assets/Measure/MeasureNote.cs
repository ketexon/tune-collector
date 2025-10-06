using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MeasureNote : MonoBehaviour
{
	[SerializeField]
	Theme theme;

	public NoteValue NoteValue;
	public Measure Measure;
	public float OffsetPercent;

	[SerializeField]
	Color missColor;

	[Header("Miss Shake")]
	[SerializeField] float shakeMagnitude = 5f;
	[SerializeField] float shakeRotateMagnitude = 5f;
	[SerializeField] float shakeDuration = 0.5f;


	[SerializeField]
	Image image;

	void Start()
	{
		var element = NoteValue.Element;
		if (element == null)
		{
			element = Measure.Pattern.DefaultElement;
		}

		Color color = theme.GetColor(element.TuneType);
		color.a = 0.5f;
		image.color = color;
	}

	public void Play()
	{
		EventBus.NotePlayedEvent.Invoke(this);
		Measure.OnNoteHit();
		image.color = new(
			image.color.r,
			image.color.g,
			image.color.b,
			1f
		);
	}

	public void Miss()
	{
		EventBus.NoteMissedEvent.Invoke(this);
		Measure.OnNoteMissed();
		GetComponentInChildren<Image>().color = missColor;
		Shake();
	}

	void Shake()
	{
		var rt = transform as RectTransform;
		var originalPos = rt.anchoredPosition;
		StartCoroutine(Impl());
		IEnumerator Impl()
		{
			float endTime = Time.time + shakeDuration;
			while (Time.time < endTime)
			{
				var offset = Random.insideUnitCircle * shakeMagnitude;
				rt.anchoredPosition = originalPos + offset;
				rt.rotation = Quaternion.Euler(0, 0, Random.Range(-shakeRotateMagnitude, shakeRotateMagnitude));
				yield return null;
			}
			rt.anchoredPosition = originalPos;
			rt.rotation = Quaternion.identity;
		}
	}
}
