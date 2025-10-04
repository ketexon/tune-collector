using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoteValue
{
    public int Duration = 1;
    public int Division = 4;
    public int Pitch = 0;

    public float DurationBeats => (float) Duration / Division;
}

[CreateAssetMenu(fileName = "Pattern", menuName = "Pattern")]
public class Pattern : ScriptableObject
{
    [SerializeField]
    public List<NoteValue> Notes;
}
