using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SheetMusic : MonoBehaviour
{
    public float BPM = 120f;
    public float BPS => BPM / 60f;

    [SerializeField]
    private RectTransform measureSlotContainer;

    [System.NonSerialized]
    public float CurMeasure = 0f;

    [System.NonSerialized]
    public bool IsPlaying = false;

    private RectTransform RectTransform => transform as RectTransform;

    float stride = 0f;

    public class NoteOffset
    {
        public MeasureNote Note;
        public float Offset;
    }

    [System.NonSerialized]
    public List<NoteOffset> NoteOffsets = new();

    int nextNodeIndex = -1;
    bool transitionedToStart = false;
    float lastMeasureEnd = 0f;

    void Start()
    {
        EventBus.PlayEvent.AddListener(OnPlay);
    }

    public void OnPlay()
    {
        CurMeasure = -1f;
        IsPlaying = true;
        ComputeStride();
        ComputeNoteTimes();
        StartCoroutine(MoveMusic(
            -CurMeasure * stride + RectTransform.rect.width / 2f
        ));
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
        nextNodeIndex = NoteOffsets.Count > 0 ? 0 : -1;
    }

    void Update()
    {
        if (IsPlaying)
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
        measureSlotContainer.anchoredPosition = targetPosition;
        transitionedToStart = true;
    }

    void UpdatePlayback()
    {
        // lerp to start position
        if (!transitionedToStart)
        {
            return;
        }

        var targetPosition = new Vector2(
            -CurMeasure * stride + RectTransform.rect.width / 2f,
            measureSlotContainer.anchoredPosition.y
        );

        CurMeasure += Time.deltaTime * BPS / 4f;
        measureSlotContainer.anchoredPosition = targetPosition;

        if (CurMeasure >= lastMeasureEnd)
        {
            IsPlaying = false;
            transitionedToStart = false;
            EventBus.SongEndedEvent.Invoke();
            ClearSlots();
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
