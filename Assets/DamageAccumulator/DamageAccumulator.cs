using System.Collections;
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

    [SerializeField]
    float dealDamageInterval = 0.2f;

    [SerializeField]
    Transform damageTarget;

    LinkedList<DamageAccumulatorEntry> entries = new();

    int entriesInQueue = 0;

    void Start()
    {
        EventBus.MeasurePassedEvent.AddListener(OnMeasurePassed);
        EventBus.SongEndedEvent.AddListener(OnSongEnded);
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
            entriesInQueue++;
            this.Delay(i * damageInterval, () => {
                entriesInQueue--;
                AddEntry(measure.transform, type);
            });
        }
    }

    void AddEntry(Transform startTransform, TuneType type)
    {
        var entry = Instantiate(entryPrefab, transform);
        entry.Type = type;
        entry.transform.position = startTransform.position;
        entries.AddLast(entry);
        RecalulateTargetPositions();
        entry.MoveToTarget();
    }

    void RecalulateTargetPositions()
    {
        Vector2 startPosition = spacing * (entries.Count - 1) / 2f * Vector2.left
            + (Vector2)transform.position;
        int i = 0;
        foreach (var entry in entries)
        {
            entry.TargetPosition = startPosition + new Vector2(i * spacing, 0f);
            i++;
        }
    }

    void OnSongEnded()
    {
        IEnumerator Impl()
        {
            while(entries.Count > 0 || entriesInQueue > 0)
            {
                if (entries.Count == 0)
                {
                    yield return null;
                    continue;
                }
                var entry = entries.First.Value;

                entries.RemoveFirst();
                RecalulateTargetPositions();

                entry.TargetPosition = damageTarget.position;
                entry.MoveToTarget();

                entry.ReachedTargetEvent.AddListener(() =>
                {
                    EventBus.DamageEvent.Invoke(entry.Type);
                    Destroy(entry.gameObject);
                });

                yield return new WaitForSeconds(dealDamageInterval);
            }
            entries.Clear();
        }
        StartCoroutine(Impl());
    }
}
