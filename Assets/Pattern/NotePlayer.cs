using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Note
{
    NoteValue value;
}

public class NotePlayer : MonoBehaviour
{
    List<Note> notes;
}
