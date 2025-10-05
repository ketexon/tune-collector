using UnityEngine;

public class Slot : MonoBehaviour
{
    const float BlockLerpSpeed = 2f;

    public bool IsOccupied => CurrentBlock != null;
    public DraggableBlock CurrentBlock { get; private set; }

    [SerializeField] public Slot nextSlot;

    public void PlaceBlock(DraggableBlock block)
    {
        // If another block is already here, clear it
        if (CurrentBlock != null)
            ClearBlock();

        CurrentBlock = block;
    }

    public void ClearBlock()
    {
        if (CurrentBlock != null)
        {
            CurrentBlock = null;
        }
    }

    private void Start() => BlockChainManager.Instance.RegisterSlot(this);
    private void OnDestroy() => BlockChainManager.Instance.UnregisterSlot(this);

	void Update()
	{
		if (CurrentBlock != null && !CurrentBlock.IsDragging)
        {
            CurrentBlock.transform.position = transform.position;
        }
	}
}
