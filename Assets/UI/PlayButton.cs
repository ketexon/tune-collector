using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlayButton : MonoBehaviour
{
    [SerializeField]
    SheetMusic sheetMusic;
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
        if (!sheetMusic.CanPlay)
        {
            Notification.Instance.Show("Add at least one tune to play!");
            return;
        }
        EventBus.PlayEvent.Invoke();
        button.interactable = false;
    }
}
