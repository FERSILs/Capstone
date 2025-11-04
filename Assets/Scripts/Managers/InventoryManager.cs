using UnityEngine;
using System.Collections.Generic;
using System.Linq; // GetSaveData를 위해 추가

/// <summary>
/// Manages inventory data. (UI is handled by InventoryUI)
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Inventory Settings")]
    public int maxSlots = 40;

    public List<InventorySlot> slots = new List<InventorySlot>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        slots.Clear();
        for (int i = 0; i < maxSlots; i++)
        {
            slots.Add(new InventorySlot());
        }
    }

    /// <summary>
    /// Adds an item to the inventory. (Called by Item.cs)
    /// </summary>
    public bool Add(ItemData itemData, int amount = 1)
    {
        if (itemData == null) return false;

        if (itemData.isStackable)
        {
            InventorySlot existingSlot = FindStackableSlot(itemData);
            if (existingSlot != null)
            {
                existingSlot.AddAmount(amount);
                Debug.Log($"[Inventory] {itemData.itemName} {amount} added (stack)");
                UpdateSlotUI(existingSlot, slots.IndexOf(existingSlot));
                return true;
            }
        }

        InventorySlot emptySlot = FindEmptySlot();
        if (emptySlot != null)
        {
            emptySlot.SetItem(itemData, amount);
            Debug.Log($"[Inventory] {itemData.itemName} {amount} added (new slot)");
            UpdateSlotUI(emptySlot, slots.IndexOf(emptySlot));
            return true;
        }

        Debug.LogWarning("[Inventory] Inventory is full.");
        return false;
    }

    /// <summary>
    /// Requests InventoryUI to update a slot at a specific index.
    /// </summary>
    private void UpdateSlotUI(InventorySlot slot, int index) // (2-argument version)
    {
        if (InventoryUI.Instance != null && index >= 0)
        {
            InventoryUI.Instance.UpdateSlotByIndex(index);
        }
    }

    private InventorySlot FindStackableSlot(ItemData itemData)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty()) continue;
            if (slot.itemData == itemData && slot.IsStackFull() == false)
            {
                return slot;
            }
        }
        return null;
    }

    private InventorySlot FindEmptySlot()
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty())
            {
                return slot;
            }
        }
        return null;
    }

    // --- Save/Load & Key Functions ---

    public bool HasKey(string keyID)
    {
        if (string.IsNullOrEmpty(keyID)) return false;
        foreach (InventorySlot slot in slots)
        {
            if (!slot.IsEmpty() &&
                slot.itemData.itemType == ItemType.Key &&
                slot.itemData.keyID == keyID)
            {
                return true;
            }
        }
        return false;
    }
    public List<SlotSaveData> GetSaveData()
    {
        List<SlotSaveData> saveData = new List<SlotSaveData>();
        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty())
            {
                saveData.Add(new SlotSaveData { itemName = null, amount = 0 });
            }
            else
            {
                saveData.Add(new SlotSaveData
                {
                    itemName = slot.itemData.name,
                    amount = slot.amount
                });
            }
        }
        return saveData;
    }

    public void LoadSaveData(List<SlotSaveData> savedSlots)
    {
        if (savedSlots.Count != slots.Count)
        {
            Debug.LogError("Saved slot count does not match current slot count!");
            return;
        }

        for (int i = 0; i < slots.Count; i++)
        {
            SlotSaveData data = savedSlots[i];
            if (string.IsNullOrEmpty(data.itemName))
            {
                slots[i].Clear();
            }
            else
            {
                ItemData itemData = Resources.Load<ItemData>($"Items/{data.itemName}");
                if (itemData != null)
                {
                    slots[i].SetItem(itemData, data.amount);
                }
                else
                {
                    Debug.LogWarning($"Item '{data.itemName}' not found in Resources/Items/. Clearing slot.");
                    slots[i].Clear();
                }
            }

            // This call now correctly finds the 2-argument UpdateSlotUI
            UpdateSlotUI(slots[i], i);
        }
        Debug.Log("[InventoryManager] Inventory loaded.");
    }
}


// ▼▼▼▼▼ (오류 해결) 'InventorySlot' 클래스 정의가 여기에 있어야 합니다 ▼▼▼▼▼
/// <summary>
/// Defines a single slot in the inventory data
/// (This is NOT a MonoBehaviour)
/// </summary>
[System.Serializable]
public class InventorySlot
{
    public ItemData itemData; // Item's ScriptableObject data
    public int amount;        // How many items are in this slot

    public InventorySlot()
    {
        Clear();
    }

    // Fills the slot with an item
    public void SetItem(ItemData data, int amt)
    {
        itemData = data;
        amount = amt;
    }

    // Clears the slot
    public void Clear()
    {
        itemData = null;
        amount = 0;
    }

    // Adds to the amount
    public void AddAmount(int amt)
    {
        amount += amt;
    }

    // Is the slot empty?
    public bool IsEmpty()
    {
        return itemData == null;
    }

    // Is the stack full?
    public bool IsStackFull()
    {
        if (IsEmpty() || !itemData.isStackable) return true;
        return amount >= itemData.maxStack;
    }
}
// ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲