using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BlockSpawner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public GameObject blockPrefab; // Prefab with DraggableBlock
    private Canvas canvas; // Canvas the block will be spawned in


    bool hovered = false;

    private InputAction clickAction;

    private void Start()
    {
        canvas = FindAnyObjectByType<Canvas>();
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

    bool rectOverlaps(RectTransform rectTrans1, RectTransform rectTrans2)
    {
        Rect rect1 = new Rect(rectTrans1.position.x, rectTrans1.position.y, rectTrans1.rect.width, rectTrans1.rect.height); ;
        Debug.Log(rect1);
        Rect rect2 = new Rect(rectTrans2.position.x, rectTrans2.position.y, rectTrans2.rect.width, rectTrans2.rect.height);
        Debug.Log(rect2);
        return rect1.Overlaps(rect2);
    }
}
