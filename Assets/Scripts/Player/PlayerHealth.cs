using UnityEngine;

namespace PlayerSystem
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        public int maxHP = 2;
        private int currentHP;
        public PlayerController controller;

        private void Awake()
        {
            controller = GetComponent<PlayerController>();
            currentHP = maxHP;

            // (추가) 시작할 때 HUD UI 갱신
            UpdateHUD();
        }

        public bool TakeDamage(int damage)
        {
            if (controller.isInvincible)
            {
                Debug.Log("무적 상태로 인해 데미지 무시!");
                return false;
            }

            currentHP -= damage;
            Debug.Log($"플레이어 체력: {currentHP}/{maxHP}");

            if (currentHP <= 0)
                Die();

            // (추가) 데미지 입을 때 HUD UI 갱신
            UpdateHUD();
            return true;
        }

        public void Heal(int amount)
        {
            currentHP = Mathf.Min(currentHP + amount, maxHP);
            Debug.Log($"체력 회복: {currentHP}/{maxHP}");

            // (추가) 회복할 때 HUD UI 갱신
            UpdateHUD();
        }

        public void IncreaseMaxHP(int amount)
        {
            maxHP += amount;
            currentHP = maxHP;
            Debug.Log($"최대 체력 증가! {maxHP}");

            // (추가) 최대 체력 증가 시 HUD UI 갱신
            UpdateHUD();
        }

        private void Die()
        {
            Debug.Log("플레이어 사망");
        }

        private void UpdateHUD()
        {
            if (PlayerHUD.Instance != null)
            {
                PlayerHUD.Instance.UpdateHP(currentHP, maxHP);
            }
        }
    }
}