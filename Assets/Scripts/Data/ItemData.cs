using UnityEngine;

// [CreateAssetMenu] 어트리뷰트를 사용하면
// 유니티 에디터의 Assets > Create 메뉴에서 이 SO를 만들 수 있습니다.
[CreateAssetMenu(fileName = "New ItemData", menuName = "Data/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public string itemName; // 아이템 이름 (툴팁용)

    [TextArea]
    public string description; // 아이템 설명 (툴팁용)

    public Sprite icon; // 인벤토리 UI에 표시될 아이콘

    [Header("타입 (참고용)")]
    // (기획안의 3번 항목: 보스 드랍템)
    // (기획안의 A항목: 소모품, 열쇠 등)
    public ItemType itemType;

    [Header("인벤토리 관리")]
    public bool isStackable = true; // 겹치기 가능 여부

    [Range(1, 999)]
    public int maxStack = 99; // 최대 스택 수량
}

// 아이템의 타입을 정의합니다. (NEXT_STEPS.md A항목)
public enum ItemType
{
    Consumable, // 소모품 (체력 회복 등)
    Key,        // 열쇠/기믹 아이템
    Quest,      // 퀘스트 아이템 (보스 드랍템 등)
    Permanent   // 영구 적용 (최대 체력 증가 등 - 1번 항목은 별도 처리함)
}