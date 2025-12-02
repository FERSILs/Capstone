// InventoryUI.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �κ��丮 UI �г� ��ü�� �����մϴ�. (���, ���� �� ����)
/// </summary>
public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("UI References")]
    public GameObject inventoryPanel; // �κ��丮 ��ü �г�
    public Transform slotContainer; // ���� ��(������)���� ������ �θ� Transform

    [Header("Prefabs")]
    public GameObject slotViewPrefab; // 1�ܰ迡�� ���� SlotView�� �پ��ִ� ������

    // ������ ��� ���� ���
    private List<InventorySlotView> slotViews = new List<InventorySlotView>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // �κ��丮 �г��� �� ���·� ����
        inventoryPanel.SetActive(false);
    }

    private void Start()
    {
        // InventoryManager�� �غ�Ǳ� ��ٸ� �� ���� ����
        InitializeSlots();
    }

    private void Update()
    {
        // 'I' Ű�� �κ��丮 ���
        if (Input.GetKeyDown(KeyCode.I))
        {
            Toggle();
        }
    }

    /// <summary>
    /// InventoryManager�� �����Ϳ� ���� UI ���� �並 �����մϴ�.
    /// </summary>
    private void InitializeSlots()
    {
        // InventoryManager�� ������ ������ ��ȸ
        foreach (InventorySlot dataSlot in InventoryManager.Instance.slots)
        {
            // 1. ���� UI ������ ����
            GameObject slotObj = Instantiate(slotViewPrefab, slotContainer);

            // 2. ���� �� ������Ʈ ��������
            InventorySlotView view = slotObj.GetComponent<InventorySlotView>();

            // 3. ���� �䰡 � �����͸� ǥ������ �����ϰ�, UI ����
            view.UpdateSlot(dataSlot);

            // 4. ���� ��Ͽ� �߰�
            slotViews.Add(view);
        }
    }

    /// <summary>
    /// �κ��丮 UI�� �Ѱ� ���ϴ�.
    /// </summary>
    public void Toggle()
    {
        bool isActive = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isActive);

        InputManager.Instance.SetInventoryState(isActive);

        // NEXT_STEPS.md D�׸�: �κ��丮 �� �� ���� �Ͻ����� (���� ����)
        // Time.timeScale = isActive ? 0f : 1f;

        // InputManager�� �ִٸ� �Է� ��� (���� ����)
        // InputManager.Instance.SetInventoryState(isActive);
    }

    /// <summary>
    /// Ư�� �ε����� ���� UI�� �����մϴ�.
    /// (InventoryManager�� ȣ���� �Լ�)
    /// </summary>
    public void UpdateSlotByIndex(int index)
    {
        if (index < 0 || index >= slotViews.Count) return;

        // InventoryManager���� �ֽ� ������ ��������
        InventorySlot updatedData = InventoryManager.Instance.slots[index];

        // �ش� UI ����
        slotViews[index].UpdateSlot(updatedData);
    }
}