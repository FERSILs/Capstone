using UnityEngine;
using PlayerSystem; // PlayerHealth 참조

[RequireComponent(typeof(Collider2D))]
public class DamageHitbox : MonoBehaviour
{
    [Header("Settings")]
    public int damageAmount = 1;
    [Tooltip("판정이 유지되는 시간 (0.1초 추천)")]
    public float activeDuration = 0.1f;

    void Start()
    {
        // 생성된 직후, 아주 짧은 시간 뒤에 스스로 사라짐 (순간 타격)
        Destroy(gameObject, activeDuration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 태그 확인
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                // 데미지 주기 (PlayerHealth가 무적 체크 등 알아서 처리함)
                health.TakeDamage(damageAmount);
            }
        }
    }
}