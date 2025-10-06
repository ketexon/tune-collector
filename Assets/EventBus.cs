using UnityEngine;
using UnityEngine.Events;

public class EventBus : MonoBehaviour
{
    public static UnityEvent PlayEvent = new();
    public static UnityEvent SongEndedEvent = new();
    public static UnityEvent TapEvent = new();

    public static UnityEvent<MeasureNote> NotePlayedEvent = new();
    public static UnityEvent<MeasureNote> NoteMissedEvent = new();

    public static UnityEvent<Measure> MeasurePassedEvent = new();
    public static UnityEvent<Measure> MeasureFailedEvent = new();

    public static UnityEvent<TuneType> DamageEvent = new();
    public static UnityEvent DamageFinishedEvent = new();

    public static UnityEvent CustomersSpawnedEvent = new();
}
