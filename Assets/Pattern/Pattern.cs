using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum PitchValue
{
    Rest,
    G4, Gs4, A4, As4, B4,
    C5, Cs5, D5, Ds5, E5, F5, Fs5, G5, Gs5, A5, As5, B5,
    C6, Cs6, D6, Ds6, E6, F6, Fs6, G6
}


[System.Serializable]
public class NoteValue
{
    const int BeatsPerMeasure = 4;


    public int Duration = 1;
    public int Division = 4;
    public PitchValue Pitch = PitchValue.Rest;
    public Element Element = null;


    public float DurationMeasures => (float)Duration / Division;
    public float DurationBeats => BeatsPerMeasure * DurationMeasures;
}

[CreateAssetMenu(fileName = "Pattern", menuName = "Pattern")]
public class Pattern : ScriptableObject
{
    [SerializeField]
    public List<NoteValue> Notes;
}
