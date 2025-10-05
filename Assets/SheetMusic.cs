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
        public Measure Measure;
        public MeasureNote Note;
        public float Offset;
    }

    [System.NonSerialized]
    public List<NoteOffset> NoteOffsets = new();

    int nextNodeIndex = -1;
    bool transitionedToStart = false;

    public void Play()
    {
        CurMeasure = -1f;
        IsPlaying = true;
        ComputeStride();
        ComputeNoteTimes();

        EventBus.PlayEvent.Invoke();
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
                    Measure = measure,
                    Note = note,
                    Offset = measureOffset + note.OffsetPercent
                };
                NoteOffsets.Add(noteTime);
            }

            measureOffset += 1f;
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

    void UpdatePlayback()
    {
        var targetPosition = new Vector2(
            -CurMeasure * stride + RectTransform.rect.width / 2f,
            measureSlotContainer.anchoredPosition.y
        );

        // lerp to start position
        if (!transitionedToStart)
        {
            measureSlotContainer.anchoredPosition = Vector2.Lerp(
                measureSlotContainer.anchoredPosition,
                targetPosition,
                Time.deltaTime * 20f
            );
            var dist = Vector2.Distance(measureSlotContainer.anchoredPosition, targetPosition);
            if (Mathf.Abs(dist) < 0.01f)
            {
                transitionedToStart = true;
                measureSlotContainer.anchoredPosition = targetPosition;
            }
            return;
        }

        CurMeasure += Time.deltaTime * BPS / 4f;
        measureSlotContainer.anchoredPosition = targetPosition;
    }
}
