using UnityEngine;
using UnityEngine.UI;

public class CurrentBeatIndicator : MonoBehaviour
{
    [SerializeField]
    Animator animator;

    static readonly int SongPlayingBool = Animator.StringToHash("playing");

    void Start()
    {
        EventBus.PlayEvent.AddListener(OnPlay);
        EventBus.SongEndedEvent.AddListener(OnSongEnded);
    }

    void OnPlay()
    {
        animator.SetBool(SongPlayingBool, true);
    }

    void OnSongEnded()
    {
        animator.SetBool(SongPlayingBool, false);
    }
}
