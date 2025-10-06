using System.Collections.Generic;
using UnityEngine;

public class DamageAccumulator : MonoBehaviour
{
    [SerializeField]
    DamageAccumulatorEntry entryPrefab;

    [SerializeField]
    float spacing = 24f;

    [SerializeField]
    float damageInterval = 0.4f;

    List<DamageAccumulatorEntry> entries = new();

    void Start()
    {
        EventBus.MeasurePassedEvent.AddListener(OnMeasurePassed);
    }

    void OnMeasurePassed(Measure measure)
    {
        List<TuneType> types = new();
        foreach (var dmg in measure.Damage)
        {
            for (int i = 0; i < dmg.Damage; i++)
            {
                types.Add(dmg.Type);
            }
        }

        types.Shuffle();
        for (int i = 0; i < types.Count; i++)
        {
            var type = types[i];
            this.Delay(i * damageInterval, () => AddEntry(measure.transform, type));
        }
    }

    void AddEntry(Transform startTransform, TuneType type)
    {
        Debug.Log("ADDING ENTYR");
        var entry = Instantiate(entryPrefab, transform);
        entry.Type = type;
        entry.transform.position = startTransform.position;
        entries.Add(entry);
        RecalulateTargetPositions();
        entry.MoveToTarget();
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
