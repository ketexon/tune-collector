using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Measure))]
public class MeasureEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Measure measure = (Measure)target;
        if (GUILayout.Button("Preview Notes"))
        {
            measure.GenerateNotes();
            EditorUtility.SetDirty(measure);
        }
    }
}

#endif

[RequireComponent(typeof(RectTransform))]
public class Measure : MonoBehaviour
{
    public Pattern Pattern;

    [SerializeField] MeasureNote notePrefab;

    [SerializeField]
    private Transform noteContainer;

    private List<MeasureNote> measureNotes = new();

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
                noteContainer.GetChild(noteContainer.childCount - 1)
                .gameObject
            );
        }
        measureNotes.Clear();

        var rectTransform = transform as RectTransform;
        float currentOffsetPercent = 0f;
        Debug.Log($"HI {Pattern.Notes.Count}");
        foreach (var noteValue in Pattern.Notes)
        {
            Debug.Log(noteValue);

            var note = Instantiate(notePrefab, noteContainer);
            note.name = $"Note {currentOffsetPercent:F2}";

            float startPos = currentOffsetPercent * rectTransform.rect.width;
            var noteRect = note.transform as RectTransform;

            noteRect.pivot = new Vector2(0, 0.5f);
            noteRect.anchoredPosition = new Vector2(startPos, 0);
            noteRect.anchorMin = new Vector2(0, 0.5f);
            noteRect.anchorMax = new Vector2(0, 0.5f);
            noteRect.sizeDelta = new Vector2(
                noteValue.DurationMeasures * rectTransform.rect.width,
                noteRect.sizeDelta.y
            );

            measureNotes.Add(note);
            currentOffsetPercent += noteValue.DurationMeasures;
        }
    }
}