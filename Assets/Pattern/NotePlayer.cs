using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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
        var element = measureNote.NoteValue.Element;
        if (element == null)
        {
            element = measureNote.Measure.Pattern.DefaultElement;
        }
        if (element == null)
        {
            Debug.LogError($"No element for note {measureNote.NoteValue}");
            return;
        }
        var instrument = element.Instrument;
        var pitch = measureNote.NoteValue.Pitch;
        var sample = instrument.GetSample(pitch);

        if (sample == null)
        {
            Debug.LogWarning($"No sample for pitch {pitch} in instrument {instrument.name}");
            return;
        }

        var audioSource = gameObject.AddComponent<AudioSource>();
        StartCoroutine(PlayNote(audioSource, sample, noteLengthSeconds + legatoDuration));
    }

    IEnumerator PlayNote(AudioSource audio, AudioClip clip, float duration)
    {
        audio.clip = clip;
        audio.Play();
        float timeElapsed = 0;
        float origVolume = audio.volume;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            audio.volume = Mathf.Lerp(origVolume, 0, t);
            yield return null;
            timeElapsed += Time.deltaTime;
        }

        Destroy(audio);
    }
}
