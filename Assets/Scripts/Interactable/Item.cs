using PlayerSystem; // IPickup 인터페이스를 사용하기 위해
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Item : MonoBehaviour, IPickup
{
    [Header("아이템 데이터")]
    [Tooltip("이 오브젝트가 줄 아이템의 ScriptableObject 데이터")]
    public ItemData itemData;
    public int amount = 1;

    // [수정] IPickup 인터페이스의 요구사항에 맞게 매개변수 추가
    public void Pickup(PlayerInteraction interactor)
    {
        if (itemData == null)
        {
            Debug.LogWarning($"{name}: ItemData가 연결되지 않아 획득할 수 없습니다.");
            return;
        }

        // --- 3번 항목(보스 드랍템) 구현 ---
        // TODO: InventoryManager.cs가 구현되면 아래 주석을 해제하세요.

        
        // InventoryManager에 아이템 추가 시도
        bool success = InventoryManager.Instance.Add(itemData, amount);

        if (success)
        {
            Debug.Log($"{itemData.itemName} {amount}개 획득!");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("인벤토리가 가득 찼습니다.");
            // (획득 실패 시 아이템을 파괴하지 않음)
        }
        

        // --- 임시 코드 (InventoryManager 구현 전) ---
        // 우선은 획득 로그만 출력하고 파괴합니다.
        // Debug.Log($"[임시] {itemData.itemName} 획득 시도!");
        // Destroy(gameObject);
        // ----------------------------------------
    }
}