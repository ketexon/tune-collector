using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRhythm : MonoBehaviour
{
	[SerializeField]
	InputActionReference tapAction;

	[SerializeField]
	SheetMusic sheetMusic;

	[SerializeField]
	float hitWindow = 1/30f;

	[SerializeField]
	float tooEarlyWindow = 3/30f;

	int currentNoteIndex = 0;

	void Start()
	{
		EventBus.PlayEvent.AddListener(OnPlay);
		enabled = false;
	}

	void OnPlay()
	{
		currentNoteIndex = 0;
		enabled = true;
	}

	void OnEnable()
	{
		tapAction.action.performed += OnTap;
	}

	void OnDisable()
	{
		tapAction.action.performed -= OnTap;
	}

	void Update()
	{
		if (!sheetMusic.IsPlaying)
			return;
		if (currentNoteIndex >= sheetMusic.NoteOffsets.Count)
			return;

		var currentNoteOffset = sheetMusic.NoteOffsets[currentNoteIndex];
		var offset = currentNoteOffset.Offset;
		var curMeasure = sheetMusic.CurMeasure;
		var diffMeasures = curMeasure - offset;
		var diffSeconds = diffMeasures * 4 / sheetMusic.BPS;

		if (diffSeconds > hitWindow)
		{
			Debug.Log($"Missed note at {offset:F2} (cur {curMeasure:F2})");
			currentNoteIndex++;

			currentNoteOffset.Note.Miss();
		}
	}

	void OnTap(InputAction.CallbackContext context)
	{
		if (!sheetMusic.IsPlaying)
			return;

		EventBus.TapEvent.Invoke();

		if (currentNoteIndex >= sheetMusic.NoteOffsets.Count)
			return;

		var currentNoteOffset = sheetMusic.NoteOffsets[currentNoteIndex];
		var offset = currentNoteOffset.Offset;
		var curMeasure = sheetMusic.CurMeasure;
		var diffMeasures = curMeasure - offset;
		var diffSeconds = diffMeasures * 4 / sheetMusic.BPS;

		if (Mathf.Abs(diffSeconds) <= hitWindow)
		{
			currentNoteIndex++;

			currentNoteOffset.Note.Play();
		}
		else if (-tooEarlyWindow <= diffSeconds && diffSeconds < -hitWindow)
		{
			currentNoteIndex++;

			currentNoteOffset.Note.Miss();
		}
		// otherwise, not in too early window so ignore
	}
}