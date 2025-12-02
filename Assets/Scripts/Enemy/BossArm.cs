using UnityEngine;
using System.Collections;

public class BossArm : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;

    void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    // BossController가 이 함수를 호출
    public void Slam(Vector3 targetPosition)
    {
        StartCoroutine(SlamCoroutine(targetPosition));
    }

    IEnumerator SlamCoroutine(Vector3 targetPosition)
    {
        // (임시) 0.5초 만에 목표 지점으로 이동
        // (TODO: 나중에 DoTween이나 애니메이션으로 대체)
        Vector3 raisePos = startPosition + new Vector3(0, 5, 0); // (예시) 팔 들어올리기

        // 1. 팔 들기
        float t = 0;
        while (t < 0.2f)
        {
            transform.position = Vector3.Lerp(startPosition, raisePos, t / 0.2f);
            t += Time.deltaTime;
            yield return null;
        }

        // 2. 내려찍기 (플레이어 추적)
        t = 0;
        while (t < 0.3f)
        {
            transform.position = Vector3.Lerp(raisePos, targetPosition, t / 0.3f);
            t += Time.deltaTime;
            yield return null;
        }

        // (이펙트)
        // CameraShake.Instance.Shake(0.2f);

        yield return new WaitForSeconds(0.5f); // 내려찍은 상태로 잠시 대기

        // 3. 원위치 복귀
        t = 0;
        while (t < 0.5f)
        {
            transform.position = Vector3.Lerp(targetPosition, startPosition, t / 0.5f);
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = startPosition;
    }
}