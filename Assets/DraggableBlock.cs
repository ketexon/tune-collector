using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool IsDragging { get; private set; } = false;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    private Slot currentSlot;
    private Slot nearestSlot;

    [SerializeField] private float snapRange = 100f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        IsDragging = true;

        // Allow re-dragging: if we were in a slot, free it first
        if (currentSlot != null)
        {
            currentSlot.ClearBlock();
            currentSlot = null;
            transform.SetParent(canvas.transform);
        }

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f; // slightly transparent while dragging
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        // Continuously find nearest slot in range
        nearestSlot = BlockChainManager.Instance.GetNearestSlot(rectTransform.position, snapRange);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDragging = false;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        if (nearestSlot != null)
        {
            // Snap to slot
            rectTransform.position = nearestSlot.transform.position;
            nearestSlot.PlaceBlock(this);
            currentSlot = nearestSlot;
            transform.SetParent(nearestSlot.transform);
        }
        else
        {
            // No slot nearby delete
            Destroy(gameObject);
        }
    }
}
