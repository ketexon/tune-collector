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
        if (GUILayout.Button("Generate Notes"))
        {
            EditorUtility.SetDirty(measure);
        }
    }
}
#endif

public class Measure : MonoBehaviour
{
    public Pattern Pattern;

    [SerializeField] float width = 4.0f;

    [SerializeField] MeasureNote notePrefab;

    [SerializeField]
    private Transform noteContainer;

    [HideInInspector]
    [SerializeField]
    private List<MeasureNote> measureNotes;

#if UNITY_EDITOR
    // called in editor only
    public void GenerateNotes()
    {
        if (Pattern == null || noteContainer == null)
        {
            Debug.LogWarning("Pattern or Note Container is not assigned.");
            return;
        }

        if (Application.isPlaying)
        {
            Debug.LogWarning("GenerateNotes should only be called in the editor.");
            return;
        }

        // Clear existing notes
        foreach (Transform child in noteContainer)
        {
            DestroyImmediate(child.gameObject);
        }
        measureNotes.Clear();

        float currentBeat = 0f;
        foreach (var noteValue in Pattern.Notes)
        {
            var note = Instantiate(notePrefab, noteContainer);
            note.name = $"Note {currentBeat:F2}";

            float startPos = (currentBeat / 4f) * width;
            note.transform.localPosition = new(startPos, 0, 0);


            measureNotes.Add(note);
            currentBeat += noteValue.DurationBeats;
        }
    }
#endif
}