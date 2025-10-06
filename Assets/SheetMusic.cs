using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameState {
    ChoosingTunes,
    TransitionToPlay,
    Playing,
    TransitionToEnd,
}

[RequireComponent(typeof(RectTransform))]
public class SheetMusic : MonoBehaviour
{
    GameState state = GameState.ChoosingTunes;
    public GameState State
    {
        get => state;
        private set
        {
            if (state == value) return;
            var old = state;
            state = value;
            StateChangedEvent.Invoke(old, state);
        }
    }

    public UnityEvent<GameState, GameState> StateChangedEvent = new();

    public bool IsPlaying => state == GameState.Playing;

    public float BPM = 120f;
    public float BPS => BPM / 60f;

    [SerializeField]
    private RectTransform measureSlotContainer;

    [SerializeField]
    AudioSource bgm;
    [SerializeField] float bgmOffset = 0.2f;

    [System.NonSerialized]
    public float CurMeasure = 0f;

    private RectTransform RectTransform => transform as RectTransform;

    float stride = 0f;

    public class NoteOffset
    {
        public MeasureNote Note;
        public float Offset;
    }

    [System.NonSerialized]
    public List<NoteOffset> NoteOffsets = new();

    float lastMeasureEnd = 0f;
    double startDspTime = 0f;
    float startMeasure = -1;

    void Start()
    {
        EventBus.PlayEvent.AddListener(OnPlay);
        StateChangedEvent.AddListener(OnStateChanged);
    }

    public void OnPlay()
    {
        CurMeasure = startMeasure;
        State = GameState.TransitionToPlay;
        ComputeStride();
        ComputeNoteTimes();
        StartCoroutine(MoveMusic(
            -CurMeasure * stride + RectTransform.rect.width / 2f
        ));
    }

    void OnStateChanged(GameState from, GameState to)
    {
        if (to == GameState.Playing)
        {
            startDspTime = AudioSettings.dspTime;
        }
    }

    // note: don't do until layout is done (after first update)
    void ComputeStride()
    {
        // get stride between measure slots
        // note: can't just use width because of spacing
        var firstSlot = measureSlotContainer.GetChild(0) as RectTransform;
        var secondSlot = measureSlotContainer.GetChild(1) as RectTransform;
        stride = secondSlot.anchoredPosition.x - firstSlot.anchoredPosition.x;
    }

    void ComputeNoteTimes()
    {
        NoteOffsets.Clear();
        float measureOffset = 0f;
        foreach (Transform measureSlotTransform in measureSlotContainer)
        {
            var slot = measureSlotTransform.GetComponent<Slot>();
            if (slot.CurrentBlock == null)
            {
                measureOffset += 1f;
                continue;
            }

            var measure = slot.CurrentBlock.GetComponent<Measure>();

            foreach (var note in measure.MeasureNotes)
            {
                var noteTime = new NoteOffset
                {
                    Note = note,
                    Offset = measureOffset + note.OffsetPercent
                };
                NoteOffsets.Add(noteTime);
            }

            measureOffset += 1f;
            lastMeasureEnd = measureOffset;
        }
    }

    void Update()
    {
        if (state == GameState.Playing)
        {
            UpdatePlayback();
        }
    }

    IEnumerator MoveMusic(float targetX)
    {
        float startTime = Time.time;
        float endTime = startTime + 0.25f;

        var startPosition = measureSlotContainer.anchoredPosition;
        var targetPosition = new Vector2(
            targetX,
            measureSlotContainer.anchoredPosition.y
        );

        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / (endTime - startTime);
            t = Mathf.SmoothStep(0f, 1f, t);

            measureSlotContainer.anchoredPosition = Vector2.Lerp(
                startPosition,
                targetPosition,
                t
            );
            yield return null;
        }
        ToggleBGM();
        yield return new WaitForSeconds(bgmOffset);
        measureSlotContainer.anchoredPosition = targetPosition;
        State = GameState.Playing;
    }

    void UpdatePlayback()
    {
        var targetPosition = new Vector2(
            -CurMeasure * stride + RectTransform.rect.width / 2f,
            measureSlotContainer.anchoredPosition.y
        );

        double dspTimeSinceStart = AudioSettings.dspTime - startDspTime;
        CurMeasure = (float) (dspTimeSinceStart * BPS / 4f) + startMeasure;
        measureSlotContainer.anchoredPosition = targetPosition;

        if (CurMeasure >= lastMeasureEnd)
        {
            State = GameState.TransitionToEnd;
            EventBus.SongEndedEvent.Invoke();
        }
    }

    void ToggleBGM(bool turningOff = false)
    {
        if (turningOff)
        {
            bgm.Stop();
        } else
        {
            bgm.Play();
        }
    }

    void ClearSlots()
    {
        foreach (Transform measureSlotTransform in measureSlotContainer)
        {
            var slot = measureSlotTransform.GetComponent<Slot>();
            if (slot.CurrentBlock != null)
            {
                Destroy(slot.CurrentBlock.gameObject);
                slot.ClearBlock();
            }
        }
    }
}
