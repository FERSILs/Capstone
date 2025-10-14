using UnityEngine;

namespace PlayerSystem
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        public int maxHP = 2;
        private int currentHP;
        private PlayerController controller;

        private void Awake()
        {
            controller = GetComponent<PlayerController>();
            currentHP = maxHP;
        }

        public void TakeDamage(int damage)
        {
            if (controller.isInvincible)
            {
                Debug.Log("무적 상태로 인해 데미지 무시!");
                return;
            }

            currentHP -= damage;
            Debug.Log($"플레이어 체력: {currentHP}/{maxHP}");

            if (currentHP <= 0)
                Die();
        }

        public void Heal(int amount)
        {
            currentHP = Mathf.Min(currentHP + amount, maxHP);
            Debug.Log($"체력 회복: {currentHP}/{maxHP}");
        }

        public void IncreaseMaxHP(int amount)
        {
            maxHP += amount;
            currentHP = maxHP;
            Debug.Log($"최대 체력 증가! {maxHP}");
        }

        private void Die()
        {
            Debug.Log("플레이어 사망");
            // GameManager를 통해 리스폰 or 게임오버 처리 가능
        }
    }
}
