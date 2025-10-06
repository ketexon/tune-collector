using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlayButton : MonoBehaviour
{
    Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnPressed);

        EventBus.CustomersSpawnedEvent.AddListener(OnCustomersSpawned);
    }

    void OnCustomersSpawned()
    {
        button.interactable = true;
    }

    void OnPressed()
    {
        EventBus.PlayEvent.Invoke();
        button.interactable = false;
    }
}
