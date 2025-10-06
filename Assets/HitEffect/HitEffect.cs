using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [SerializeField]
    Animator animator;

    static readonly int HitPercussionTrigger = Animator.StringToHash("hit_percussion");
    static readonly int HitMelodyTrigger = Animator.StringToHash("hit_melody");
    static readonly int HitBassTrigger = Animator.StringToHash("hit_bass");

    void Start()
    {
        EventBus.NotePlayedEvent.AddListener(OnNotePlayed);
    }

    void OnNotePlayed(MeasureNote note)
    {
        var element = note.NoteValue.Element;
        if (element == null)
        {
            element = note.Measure.Pattern.DefaultElement;
        }
        animator.SetTrigger(element.TuneType switch
        {
            TuneType.Melody => HitMelodyTrigger,
            TuneType.Bass => HitBassTrigger,
            TuneType.Percussion => HitPercussionTrigger,
            _ => -1,
        });
    }
}
