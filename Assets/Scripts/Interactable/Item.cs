// Item.cs
using PlayerSystem;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Item : MonoBehaviour, IPickup
{
    [Header("Item Data")]
    public ItemData itemData;
    public int amount = 1;

    [Header("Save/Load Settings")]
    [Tooltip("이 아이템을 식별할 고유 ID (씬에서 유일해야 함)")]
    public string uniqueItemID; // 예: "Miro1_BossKey"

    void Start()
    {
        // 씬 로드 시, 이 아이템이 이미 플래그에 저장(획득)되었는지 확인
        if (!string.IsNullOrEmpty(uniqueItemID) && FlagManager.Instance.CheckFlag(uniqueItemID))
        {
            gameObject.SetActive(false); // 이미 먹은 템이면 숨김
        }
    }

    public void Pickup(PlayerInteraction interactor)
    {
        if (itemData == null)
        {
            Debug.LogWarning($"{name}: ItemData가 없습니다.");
            return;
        }

        bool success = InventoryManager.Instance.Add(itemData, amount);

        if (success)
        {
            Debug.Log($"{itemData.itemName} {amount}개 획득!");

            // (수정) 1회성 아이템이면 플래그 저장
            if (!string.IsNullOrEmpty(uniqueItemID))
            {
                FlagManager.Instance.SetFlag(uniqueItemID);
            }

            // (수정) Destroy 대신 SetActive(false) 사용
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("인벤토리가 가득 찼습니다.");
        }
    }
}