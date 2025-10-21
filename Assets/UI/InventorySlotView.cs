// InventorySlotView.cs
using UnityEngine;
using UnityEngine.UI; // Image를 사용하기 위해
using TMPro; // TextMeshPro를 사용하기 위해

/// <summary>
/// 인벤토리 슬롯 '하나'의 UI 표시를 담당합니다.
/// (이 스크립트는 '슬롯 프리팹'에 붙습니다)
/// </summary>
public class InventorySlotView : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI amountText;

    private InventorySlot currentSlotData; // 이 UI가 현재 표시 중인 데이터

    /// <summary>
    /// 이 슬롯 UI를 특정 데이터로 갱신합니다.
    /// (InventoryUI가 이 함수를 호출합니다)
    /// </summary>
    public void UpdateSlot(InventorySlot slotData)
    {
        currentSlotData = slotData;

        if (currentSlotData.IsEmpty())
        {
            // 1. 슬롯이 비어있을 때
            iconImage.sprite = null;
            iconImage.color = new Color(0, 0, 0, 0); // 완전 투명
            amountText.text = "";
        }
        else
        {
            // 2. 슬롯에 아이템이 있을 때
            iconImage.sprite = currentSlotData.itemData.icon;
            iconImage.color = Color.white; // 불투명

            // 3. 수량 표시 (겹칠 수 있고 1개보다 많을 때만)
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