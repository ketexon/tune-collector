using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Measure : MonoBehaviour
{
    const float PassPercent = 0.1f;
    public string TuneName;
    public Pattern Pattern;
    public List<TuneTypeDamage> Damage;

    [System.NonSerialized]
    public List<MeasureNote> MeasureNotes = new();

    [SerializeField] MeasureNote notePrefab;
	[SerializeField] private Transform noteContainer;

    int notesHit = 0;
    int notesMissed = 0;

    float CurPercent => notesHit / (float)MeasureNotes.Count;

    void Start()
    {
        GenerateNotes();
    }

    public void GenerateNotes()
    {
        if (Pattern == null || noteContainer == null)
        {
            Debug.LogWarning("Pattern or Note Container is not assigned.");
            return;
        }


        // Clear existing notes
        while (noteContainer.childCount > 0)
        {
            DestroyImmediate(
                noteContainer
					.GetChild(noteContainer.childCount - 1)
                	.gameObject
            );
        }
        MeasureNotes.Clear();

        var rectTransform = transform as RectTransform;
        float currentOffsetPercent = 0f;
        foreach (var noteValue in Pattern.Notes)
        {
            if (noteValue.Pitch == PitchValue.Rest)
            {
                currentOffsetPercent += noteValue.DurationMeasures;
                continue;
            }

            var note = Instantiate(notePrefab, noteContainer);
            note.NoteValue = noteValue;
            note.Measure = this;
            note.OffsetPercent = currentOffsetPercent;
            note.name = $"Note {currentOffsetPercent:F2}";

            var endAnchorX = currentOffsetPercent + noteValue.DurationMeasures;
            var noteRect = note.transform as RectTransform;

            noteRect.pivot = new Vector2(0, 0.5f);
            noteRect.anchoredPosition = new Vector2(0, 0);
            noteRect.anchorMin = new Vector2(currentOffsetPercent, 0.5f);
            noteRect.anchorMax = new Vector2(endAnchorX, 0.5f);
            noteRect.sizeDelta = new Vector2(
                0,
                noteRect.sizeDelta.y
            );

            MeasureNotes.Add(note);
            currentOffsetPercent += noteValue.DurationMeasures;
        }
    }

    public void OnNoteHit()
    {
        ++notesHit;
        if (IsComplete)
        {
            OnMeasureComplete();
        }
    }

    public void OnNoteMissed()
    {
        ++notesMissed;
        if (IsComplete)
        {
            OnMeasureComplete();
        }
    }

    public void OnMeasureComplete()
    {
        if (CurPercent >= PassPercent)
        {
            EventBus.MeasurePassedEvent.Invoke(this);
        }
        else
        {
            EventBus.MeasureFailedEvent.Invoke(this);
        }
    }

    bool IsComplete => notesHit + notesMissed >= MeasureNotes.Count;
}