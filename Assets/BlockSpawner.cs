using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BlockSpawner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public GameObject blockPrefab; // Prefab with DraggableBlock
    [SerializeField] Measure rendererMeasure;
    private Canvas canvas; // Canvas the block will be spawned in

    bool hovered = false;

    private InputAction clickAction;

    private void Start()
    {
        canvas = FindAnyObjectByType<Canvas>();
        var prefabMeasure = blockPrefab.GetComponent<Measure>();
        if (rendererMeasure)
        {
            rendererMeasure.Pattern = prefabMeasure.Pattern;
            rendererMeasure.GenerateNotes();
        }
    }

    private void OnEnable()
    {
        clickAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/leftButton");
        clickAction.performed += OnClick;
        clickAction.Enable();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!hovered)
        {
            return;
        }
        GameObject newBlock = Instantiate(blockPrefab, canvas.transform);
        RectTransform rect = newBlock.GetComponent<RectTransform>();
        rect.position = gameObject.GetComponent<RectTransform>().position;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
    }
}
