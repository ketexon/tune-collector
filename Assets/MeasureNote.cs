using UnityEngine;
using UnityEngine.UI;

public class MeasureNote : MonoBehaviour
{
	public NoteValue NoteValue;
	public Measure Measure;
	public float OffsetPercent;

	public void Play()
	{
		EventBus.NotePlayedEvent.Invoke(this);
		GetComponentInChildren<Image>().color = Color.green;
	}

	public void Miss()
	{
		EventBus.NoteMissedEvent.Invoke(this);
		GetComponentInChildren<Image>().color = Color.red;
	}
}
