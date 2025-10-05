using UnityEngine;
using UnityEngine.Events;

public class EventBus : MonoBehaviour
{
    public static UnityEvent PlayEvent = new();
    public static UnityEvent TapEvent = new();
}
