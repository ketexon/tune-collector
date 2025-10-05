using UnityEngine;
using System.Collections.Generic;

public class NotePlayer : MonoBehaviour
{
    [SerializeField]
    SheetMusic sheetMusic;

    [SerializeField]
    float legatoDuration = 0.1f;

    void Start()
    {
        EventBus.NotePlayedEvent.AddListener(OnNotePlayed);
    }

    void OnNotePlayed(MeasureNote measureNote)
    {
        var noteLengthSeconds = measureNote.NoteValue.DurationBeats / sheetMusic.BPS;
        var instrument = measureNote.NoteValue.Element.Instrument;
        var pitch = measureNote.NoteValue.Pitch;
        var sample = instrument.GetSample(pitch);

        if (sample == null)
        {
            Debug.LogWarning($"No sample for pitch {pitch} in instrument {instrument.name}");
            return;
        }

        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = sample;
        audioSource.Play();

        Destroy(audioSource, noteLengthSeconds + legatoDuration);
    }
}
