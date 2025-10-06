using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PatternPlayer : MonoBehaviour
{
	[SerializeField]
	float bpm = 120f;
	float BPS => bpm / 60f;

	[SerializeField]
	float legatoDuration = 0.1f;


	[SerializeField]
	public Pattern Pattern;

	[SerializeField]
	public Instrument Instrument;

	[System.NonSerialized]
	public UnityEvent PlaybackFinishedEvent = new();

	double startDspTime = 0f;

	[System.NonSerialized]
	public float CurMeasure = 0f;

	int nextNote = 0;
	double nextNoteTime = 0f;

	void Start()
	{
		startDspTime = AudioSettings.dspTime;
		nextNoteTime = startDspTime;
	}

	void Update()
	{
		double dspTimeSinceStart = AudioSettings.dspTime - startDspTime;
		CurMeasure = (float)(dspTimeSinceStart * BPS / 4f);
		if (CurMeasure >= 1)
		{
			PlaybackFinishedEvent.Invoke();
			Destroy(this);
			return;
		}

		if (Pattern != null)
		{
			while (nextNote < Pattern.Notes.Count && nextNoteTime <= AudioSettings.dspTime)
			{
				var noteValue = Pattern.Notes[nextNote];
				if (noteValue.Pitch != PitchValue.Rest)
				{
					PlayNote(noteValue);
				}

				nextNoteTime += noteValue.DurationBeats / BPS;
				nextNote++;
			}
		}
	}

	void PlayNote(NoteValue noteValue)
	{
		var noteLengthSeconds = noteValue.DurationBeats / BPS;
		var element = noteValue.Element;
		if (element == null)
		{
			element = Pattern.DefaultElement;
		}
		var instrument = element.Instrument;
		var pitch = noteValue.Pitch;
		var sample = instrument.GetSample(pitch);

		if (sample == null)
		{
			Debug.LogWarning($"No sample for pitch {pitch} in instrument {instrument.name}");
			return;
		}

		StartCoroutine(Impl());

		IEnumerator Impl()
		{
			var audio = gameObject.AddComponent<AudioSource>();
			audio.clip = sample;
			audio.Play();
			float timeElapsed = 0;
			float origVolume = audio.volume;

			while (timeElapsed < noteLengthSeconds + legatoDuration)
			{
				float t = timeElapsed / (noteLengthSeconds + legatoDuration);
				audio.volume = Mathf.Lerp(origVolume, 0, t);
				yield return null;
				timeElapsed += Time.deltaTime;
			}

			Destroy(audio);
		}
    }
}