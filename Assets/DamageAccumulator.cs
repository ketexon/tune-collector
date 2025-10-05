using UnityEngine;

public class DamageAccumulator : MonoBehaviour
{
    [SerializeField]
    GameObject prefab;

    void Start()
    {
        EventBus.MeasurePassedEvent.AddListener(OnMeasurePassed);
    }

    void OnMeasurePassed(Measure measure)
    {
        var obj = Instantiate(prefab, transform);

    }
}
