using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BlockChainManager : MonoBehaviour
{
    public static BlockChainManager Instance { get; private set; }

    private List<Slot> slots = new List<Slot>();
    [SerializeField] Slot firstSlot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterSlot(Slot slot)
    {
        if (!slots.Contains(slot))
            slots.Add(slot);
    }

    public void UnregisterSlot(Slot slot)
    {
        if (slots.Contains(slot))
            slots.Remove(slot);
    }

    public Slot GetNearestSlot(Vector3 position, float range)
    {
        Slot nearest = null;
        float minDist = float.MaxValue;

        foreach (var slot in slots.Where(s => !s.IsOccupied))
        {
            float dist = Vector3.Distance(slot.GetComponent<RectTransform>().position, position);
            if (dist < minDist && dist < range)
            {
                minDist = dist;
                nearest = slot;
            }
        }

        return nearest;
    }

    public List<GameObject> ProcessSlots()
    {
        Slot cur = firstSlot;
        List<GameObject> output = new List<GameObject>();
        while (cur != null)
        {
            if (cur.CurrentBlock != null)
            {
                output.Add(cur.CurrentBlock.gameObject);
            }
            cur = cur.nextSlot;
        }
        return output;
    }
}
