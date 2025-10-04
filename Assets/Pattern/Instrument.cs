using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Instrument", menuName = "Instrument")]
public class Instrument : ScriptableObject
{
    public List<AudioClip> Samples;
}
