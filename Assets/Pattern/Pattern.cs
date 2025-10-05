using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoteValue
{
    const int BeatsPerMeasure = 4;

    public int Duration = 1;
    public int Division = 4;
    public int Pitch = 0;

    public float DurationMeasures => (float) Duration / Division;
    public float DurationBeats => BeatsPerMeasure * DurationMeasures;
}

[CreateAssetMenu(fileName = "Pattern", menuName = "Pattern")]
public class Pattern : ScriptableObject
{
    [SerializeField]
    public List<NoteValue> Notes;
}
