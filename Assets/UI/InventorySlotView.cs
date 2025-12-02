// InventorySlotView.cs
using UnityEngine;
using UnityEngine.UI; // Image�� ����ϱ� ����
using TMPro; // TextMeshPro�� ����ϱ� ����

/// <summary>
/// �κ��丮 ���� '�ϳ�'�� UI ǥ�ø� ����մϴ�.
/// (�� ��ũ��Ʈ�� '���� ������'�� �ٽ��ϴ�)
/// </summary>
public class InventorySlotView : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI amountText;

    private InventorySlot currentSlotData; // �� UI�� ���� ǥ�� ���� ������

    /// <summary>
    /// �� ���� UI�� Ư�� �����ͷ� �����մϴ�.
    /// (InventoryUI�� �� �Լ��� ȣ���մϴ�)
    /// </summary>
    public void UpdateSlot(InventorySlot slotData)
    {
        currentSlotData = slotData;

        if (currentSlotData.IsEmpty())
        {
            // 1. ������ ������� ��
            iconImage.sprite = null;
            iconImage.color = new Color(0, 0, 0, 0); // ���� ����
            amountText.text = "";
        }
        else
        {
            // 2. ���Կ� �������� ���� ��
            iconImage.sprite = currentSlotData.itemData.icon;
            iconImage.color = Color.white; // ������

            // 3. ���� ǥ�� (��ĥ �� �ְ� 1������ ���� ����)
            if (currentSlotData.itemData.isStackable && currentSlotData.amount > 1)
            {
                amountText.text = $"x{currentSlotData.amount}";
            }
            else
            {
                amountText.text = "";
            }
        }
    }
}