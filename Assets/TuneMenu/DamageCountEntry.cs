using UnityEngine;

public class DamageCountEntry : MonoBehaviour
{
    public TuneType Type;
    public int Damage;

    [SerializeField]
    TMPro.TMP_Text text;

    [SerializeField]
    Transform iconContainer;

    [SerializeField]
    GameObject melodyIconPrefab;

    [SerializeField]
    GameObject bassIconPrefab;

    [SerializeField]
    GameObject percussionIconPrefab;

    void Start()
    {
        text.text = Damage.ToString();
        GameObject iconPrefab = Type switch
        {
            TuneType.Melody => melodyIconPrefab,
            TuneType.Bass => bassIconPrefab,
            TuneType.Percussion => percussionIconPrefab,
            _ => null,
        };
        if (iconPrefab != null)
        {
            var icon = Instantiate(iconPrefab, iconContainer);
            var iconRect = icon.GetComponent<RectTransform>();

            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.anchoredPosition = Vector2.zero;
            iconRect.sizeDelta = Vector2.zero;
        }
	}
}
