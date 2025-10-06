using UnityEngine;

[CreateAssetMenu(fileName = "Theme", menuName = "Theme")]
public class Theme : ScriptableObject
{
    public Color MelodyColor;
    public Color PercussionColor;
    public Color BassColor;

    public Color GetColor(TuneType tuneType) => tuneType switch
    {
        TuneType.Melody => MelodyColor,
        TuneType.Percussion => PercussionColor,
        TuneType.Bass => BassColor,
        _ => Color.white,
    };
}
