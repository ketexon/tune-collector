using UnityEngine;
using UnityEngine.UI;

public class MeasureNote : MonoBehaviour
{
	public NoteValue NoteValue;
	public Measure Measure;

	public void Play()
	{
		GetComponentInChildren<Image>().color = Color.green;
	}

	public void Miss()
	{
		GetComponentInChildren<Image>().color = Color.red;
	}
}
