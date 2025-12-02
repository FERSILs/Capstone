using UnityEngine;
using System.Collections;

public class WarningSignAndRockfall : MonoBehaviour
{
    public float warningDuration = 2.0f;
    public GameObject rockfallHitboxPrefab; // 낙석 공격 판정 프리팹

    void Start()
    {
        StartCoroutine(RockfallCoroutine());
    }

    IEnumerator RockfallCoroutine()
    {
        // 1. 2초간 경고 (워닝사인 스프라이트가 보임)
        yield return new WaitForSeconds(warningDuration);

        // 2. 낙석 공격 판정 스폰
        if (rockfallHitboxPrefab != null)
        {
            Instantiate(rockfallHitboxPrefab, transform.position, Quaternion.identity);
        }

        // 3. 워닝사인 자신은 파괴
        Destroy(gameObject);
    }
}