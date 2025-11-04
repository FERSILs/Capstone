using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; // List를 사용하기 위해 추가

/// <summary>
/// Player의 HP(아이콘)와 구르기 쿨타임 등 상태 UI를 관리합니다.
/// </summary>
public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD Instance;

    [Header("HP Settings")]
    [Tooltip("HP 아이콘 프리팹 (HPIcon_Prefab)")]
    public GameObject hpIconPrefab;
    [Tooltip("HP 아이콘들이 생성될 부모 Transform (HPContainer)")]
    public Transform hpContainer;

    [Header("Roll Cooldown")]
    [Tooltip("구르기 쿨타임을 표시할 Image 컴포넌트 (Image Type: Filled)")]
    public Image rollCooldownFill;

    // 생성된 HP 아이콘들을 관리할 리스트
    private List<GameObject> hpIcons = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// (수정) HP 아이콘을 현재 체력과 최대 체력에 맞게 갱신합니다.
    /// </summary>
    public void UpdateHP(int currentHP, int maxHP)
    {
        if (hpIconPrefab == null || hpContainer == null) return;

        // 1. 최대 체력(maxHP)만큼 아이콘이 생성되어 있는지 확인
        if (hpIcons.Count < maxHP)
        {
            // 부족한 만큼 아이콘 생성
            for (int i = hpIcons.Count; i < maxHP; i++)
            {
                GameObject newIcon = Instantiate(hpIconPrefab, hpContainer);
                hpIcons.Add(newIcon);
            }
        }

        // 2. 현재 체력(currentHP)에 맞춰 아이콘 활성화/비활성화
        for (int i = 0; i < hpIcons.Count; i++)
        {
            if (i < maxHP)
            {
                // 최대 체력 이내의 아이콘들
                bool shouldBeActive = (i < currentHP);
                hpIcons[i].SetActive(shouldBeActive);
            }
            else
            {
                // (최대 체력이 줄어든 경우) 남는 아이콘은 비활성화
                hpIcons[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// 구르기 쿨타임 바의 Fill Amount를 업데이트합니다. (0.0 ~ 1.0)
    /// </summary>
    public void UpdateRollCooldown(float percentage)
    {
        if (rollCooldownFill != null)
        {
            rollCooldownFill.fillAmount = percentage;
        }
    }
}