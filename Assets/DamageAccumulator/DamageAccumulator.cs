using System.Collections.Generic;
using UnityEngine;

public class DamageAccumulator : MonoBehaviour
{
    [SerializeField]
    DamageAccumulatorEntry entryPrefab;

    [SerializeField]
    float spacing = 24f;

    List<DamageAccumulatorEntry> entries = new();

    void Start()
    {
        EventBus.MeasurePassedEvent.AddListener(OnMeasurePassed);
    }

    void OnMeasurePassed(Measure measure)
    {
        var obj = Instantiate(entryPrefab, transform);
        obj.transform.position = measure.transform.position;
        entries.Add(obj);
        RecalulateTargetPositions();
    }

    void RecalulateTargetPositions()
    {
        Vector2 startPosition = spacing * (entries.Count - 1) / 2f * Vector2.left
            + (Vector2)transform.position;
        for (int i = 0; i < entries.Count; i++)
        {
            entries[i].TargetPosition = startPosition + new Vector2(i * spacing, 0f);
        }
    }
}
