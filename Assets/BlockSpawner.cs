using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] public GameObject blockPrefab; // Prefab with DraggableBlock
    [SerializeField] Measure rendererMeasure;
    [SerializeField] Button button;
    private Canvas canvas; // Canvas the block will be spawned in

    private void Start()
    {
        canvas = FindAnyObjectByType<Canvas>();
        var prefabMeasure = blockPrefab.GetComponent<Measure>();
        if (rendererMeasure)
        {
            rendererMeasure.Pattern = prefabMeasure.Pattern;
            rendererMeasure.GenerateNotes();
        }

        button.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        GameObject newBlock = Instantiate(blockPrefab, canvas.transform);
        RectTransform rect = newBlock.GetComponent<RectTransform>();
        rect.position = transform.position;
    }
}
