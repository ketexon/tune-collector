using System.Collections.Generic;
using UnityEngine;

public class SheetMusic : MonoBehaviour
{
    public float BPM = 120f;
    public float BPS => BPM / 60f;

    public List<Pattern> Patterns;

    [System.NonSerialized]
    public float CurBeat = 0f;

    [System.NonSerialized]
    public bool IsPlaying = false;

    void StartPlaying()
    {
        CurBeat = -4f;
        IsPlaying = true;
    }

    void Update()
    {
        if (IsPlaying)
        {
            UpdatePlayback();
        }
    }

    void UpdatePlayback()
    {
        CurBeat += Time.deltaTime * BPS;
    }
}
