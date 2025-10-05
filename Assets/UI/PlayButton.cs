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
    }

    void OnPressed()
    {
        EventBus.PlayEvent.Invoke();
        button.interactable = false;
    }
}
