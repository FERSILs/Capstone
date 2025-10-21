using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 인벤토리 데이터를 관리합니다. (UI는 InventoryUI가 담당)
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    // TODO: 인벤토리 UI 스크립트와 연결
    public InventoryUI inventoryUI; 

    [Header("Inventory Settings")]
    public int maxSlots = 40; // 인벤토리 슬롯 최대 개수

    // 1. 인벤토리 슬롯 리스트
    // (실제 데이터는 이 리스트가 관리합니다)
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

        // 인벤토리 슬롯 리스트 초기화
        for (int i = 0; i < maxSlots; i++)
        {
            slots.Add(new InventorySlot());
        }
    }

    /// <summary>
    /// 아이템을 인벤토리에 추가합니다. (Item.cs가 호출)
    /// </summary>
    /// <returns>아이템 추가 성공 여부</returns>
    public bool Add(ItemData itemData, int amount = 1)
    {
        if (itemData == null) return false;

        // 1. 겹치기 가능한 아이템(isStackable)인지 확인
        if (itemData.isStackable)
        {
            // 2. 이미 인벤토리에 같은 아이템이 있는지 검색
            InventorySlot existingSlot = FindStackableSlot(itemData);
            if (existingSlot != null)
            {
                // 3. 찾았으면 해당 슬롯에 수량(amount) 추가
                existingSlot.AddAmount(amount);
                Debug.Log($"[Inventory] {itemData.itemName} {amount}개 추가 (스택)");
                InventoryUI.Instance.UpdateSlotByIndex(slots.IndexOf(existingSlot));
                return true;
            }
        }

        // 4. 겹칠 수 없거나, 겹칠 슬롯이 없는 경우: 비어있는 슬롯 검색
        InventorySlot emptySlot = FindEmptySlot();
        if (emptySlot != null)
        {
            // 5. 비어있는 슬롯에 아이템 정보와 수량 할당
            emptySlot.SetItem(itemData, amount);
            Debug.Log($"[Inventory] {itemData.itemName} {amount}개 추가 (새 슬롯)");
            InventoryUI.Instance.UpdateSlotByIndex(slots.IndexOf(emptySlot));
            return true;
        }

        // 6. 모든 슬롯이 가득 참
        Debug.LogWarning("[Inventory] 인벤토리가 가득 찼습니다.");
        return false;
    }

    /// <summary>
    /// 해당하는 슬롯의 UI를 갱신하도록 InventoryUI에 요청합니다.
    /// </summary>
    private void UpdateSlotUI(InventorySlot slot)
    {
        if (InventoryUI.Instance != null)
        {
            int index = slots.IndexOf(slot);
            if (index >= 0)
            {
                InventoryUI.Instance.UpdateSlotByIndex(index);
            }
        }
    }

    // (Add 함수를 돕는 도우미 함수들)
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
}


/// <summary>
/// 인벤토리의 각 '칸(Slot)'을 정의하는 데이터 클래스
/// (MonoBehaviour가 아님)
/// </summary>
[System.Serializable]
public class InventorySlot
{
    public ItemData itemData; // 슬롯에 담긴 아이템의 원본 데이터
    public int amount;        // 슬롯에 담긴 아이템의 개수

    public InventorySlot()
    {
        Clear();
    }

    // 슬롯을 아이템으로 채움
    public void SetItem(ItemData data, int amt)
    {
        itemData = data;
        amount = amt;
    }

    // 슬롯을 비움
    public void Clear()
    {
        itemData = null;
        amount = 0;
    }

    // 슬롯에 아이템 수량 추가
    public void AddAmount(int amt)
    {
        amount += amt;
    }

    // 비어있는 슬롯인가?
    public bool IsEmpty()
    {
        return itemData == null;
    }

    // 이 슬롯이 꽉 찼는가?
    public bool IsStackFull()
    {
        if (IsEmpty() || !itemData.isStackable) return true; // 겹칠 수 없으면 항상 꽉 찬 상태
        return amount >= itemData.maxStack;
    }
}