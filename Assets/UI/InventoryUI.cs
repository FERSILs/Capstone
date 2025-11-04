// InventoryUI.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인벤토리 UI 패널 전체를 관리합니다. (토글, 슬롯 뷰 갱신)
/// </summary>
public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("UI References")]
    public GameObject inventoryPanel; // 인벤토리 전체 패널
    public Transform slotContainer; // 슬롯 뷰(프리팹)들이 생성될 부모 Transform

    [Header("Prefabs")]
    public GameObject slotViewPrefab; // 1단계에서 만든 SlotView가 붙어있는 프리팹

    // 관리할 모든 슬롯 뷰들
    private List<InventorySlotView> slotViews = new List<InventorySlotView>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 인벤토리 패널을 끈 상태로 시작
        inventoryPanel.SetActive(false);
    }

    private void Start()
    {
        // InventoryManager가 준비되길 기다린 후 슬롯 생성
        InitializeSlots();
    }

    private void Update()
    {
        // 'I' 키로 인벤토리 토글
        if (Input.GetKeyDown(KeyCode.I))
        {
            Toggle();
        }
    }

    /// <summary>
    /// InventoryManager의 데이터에 맞춰 UI 슬롯 뷰를 생성합니다.
    /// </summary>
    private void InitializeSlots()
    {
        // InventoryManager의 데이터 슬롯을 순회
        foreach (InventorySlot dataSlot in InventoryManager.Instance.slots)
        {
            // 1. 슬롯 UI 프리팹 생성
            GameObject slotObj = Instantiate(slotViewPrefab, slotContainer);

            // 2. 슬롯 뷰 컴포넌트 가져오기
            InventorySlotView view = slotObj.GetComponent<InventorySlotView>();

            // 3. 슬롯 뷰가 어떤 데이터를 표시할지 연결하고, UI 갱신
            view.UpdateSlot(dataSlot);

            // 4. 관리 목록에 추가
            slotViews.Add(view);
        }
    }

    /// <summary>
    /// 인벤토리 UI를 켜고 끕니다.
    /// </summary>
    public void Toggle()
    {
        bool isActive = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isActive);

        InputManager.Instance.SetInventoryState(isActive);

        // NEXT_STEPS.md D항목: 인벤토리 열 때 게임 일시정지 (선택 사항)
        // Time.timeScale = isActive ? 0f : 1f;

        // InputManager가 있다면 입력 잠금 (선택 사항)
        // InputManager.Instance.SetInventoryState(isActive);
    }

    /// <summary>
    /// 특정 인덱스의 슬롯 UI만 갱신합니다.
    /// (InventoryManager가 호출할 함수)
    /// </summary>
    public void UpdateSlotByIndex(int index)
    {
        if (index < 0 || index >= slotViews.Count) return;

        // InventoryManager에서 최신 데이터 가져오기
        InventorySlot updatedData = InventoryManager.Instance.slots[index];

        // 해당 UI 갱신
        slotViews[index].UpdateSlot(updatedData);
    }
}