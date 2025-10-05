using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Instrument", menuName = "Instrument")]
public class Instrument : ScriptableObject
{
    [System.Serializable]
    public class PitchSample
    {
        public PitchValue Pitch;
        public AudioClip Sample;
    }

    public List<PitchSample> PitchSamples;

    private Dictionary<PitchValue, AudioClip> pitchToSample = null;
    void CalculatePitchToSampleMap()
    {
        if (pitchToSample == null || pitchToSample.Count != PitchSamples.Count)
        {
            pitchToSample = new Dictionary<PitchValue, AudioClip>();
            foreach (var ps in PitchSamples)
            {
                pitchToSample[ps.Pitch] = ps.Sample;
            }
        }
    }

    public AudioClip GetSample(PitchValue pitch)
    {
        CalculatePitchToSampleMap();
        if (pitchToSample.TryGetValue(pitch, out var sample))
        {
            return sample;
        }
        return null;
    }

#if UNITY_EDITOR
    void Reset()
    {
        pitchToSample = null;

        var assetPath = AssetDatabase.GetAssetPath(this);
        if (string.IsNullOrEmpty(assetPath))
        {
            return;
        }
        var assetDir = System.IO.Path.GetDirectoryName(assetPath);
        var samples = AssetDatabase.FindAssets("t:AudioClip", new[] { assetDir });

        var filenameRe = @"^.*([A-G]#?\d)$";

        PitchSamples = new List<PitchSample>();
        foreach (var sampleGuid in samples)
        {
            var samplePath = AssetDatabase.GUIDToAssetPath(sampleGuid);
            var sample = AssetDatabase.LoadAssetAtPath<AudioClip>(samplePath);
            if (sample == null) continue;

            var name = System.IO.Path.GetFileNameWithoutExtension(samplePath);
            var match = System.Text.RegularExpressions.Regex.Match(name, filenameRe);
            if (!match.Success)
            {
                Debug.LogWarning($"Sample name does not match expected pattern: {name}");
                continue;
            }
            var pitchName = match.Groups[1].Value.Replace("#", "s");
            if (System.Enum.TryParse<PitchValue>(pitchName, out var pitch))
            {
                PitchSamples.Add(new PitchSample
                {
                    Pitch = pitch,
                    Sample = sample
                });
            }
            else
            {
                Debug.LogWarning($"Could not parse pitch from sample name: {name}");
            }
        }
    }
#endif
}
