using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [SerializeField]
    Animator animator;

    static readonly int HitTrigger = Animator.StringToHash("hit");

    void Start()
    {
        EventBus.NotePlayedEvent.AddListener(OnNotePlayed);
    }

    void OnNotePlayed(MeasureNote note)
    {
        animator.SetTrigger(HitTrigger);
    }
}
