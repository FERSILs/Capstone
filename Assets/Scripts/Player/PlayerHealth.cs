using UnityEngine;

namespace PlayerSystem
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        public int maxHP = 2;
        private int currentHP;
        public PlayerController controller; // 'private' -> 'public'으로 변경

        private void Awake()
        {
            controller = GetComponent<PlayerController>();
            currentHP = maxHP;
        }

        // ▼▼▼▼▼ 수정된 부분 (void -> bool) ▼▼▼▼▼
        /// <summary>
        /// 플레이어에게 데미지를 시도하고, 성공 여부를 반환합니다.
        /// </summary>
        /// <returns>데미지를 입었으면 true, 무적이면 false</returns>
        public bool TakeDamage(int damage)
        {
            if (controller.isInvincible)
            {
                Debug.Log("무적 상태로 인해 데미지 무시!");
                return false; // 데미지 실패 (무적)
            }
            // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

            currentHP -= damage;
            Debug.Log($"플레이어 체력: {currentHP}/{maxHP}");

            if (currentHP <= 0)
                Die();

            return true; // 데미지 성공
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