using UnityEngine;
using System.Collections;
using PlayerSystem;

// (수정) BoxCollider2D -> PolygonCollider2D로 변경
[RequireComponent(typeof(LineRenderer), typeof(PolygonCollider2D))]
public class LaserBeam : MonoBehaviour
{
    [Header("Settings")]
    public float laserLength = 20.0f; // 레이저 길이

    [Header("Shape Settings")]
    public float startWidth = 0.5f;   // 시작점(눈) 두께
    public float endWidth = 10.0f;    // 끝점(바닥) 두께 (넓게 펴짐)

    [Header("Timing")]
    public float warningTime = 1.5f;
    public float fireDuration = 3.0f;
    public int damage = 1;

    [Header("Visuals")]
    public Color warningColor = new Color(1, 0.92f, 0.016f, 0.3f); // 반투명 노랑
    public Color fireColor = new Color(1, 0, 0, 0.8f); // 진한 빨강

    private LineRenderer lineRenderer;
    private PolygonCollider2D polyCollider;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        polyCollider = GetComponent<PolygonCollider2D>(); // (수정)

        lineRenderer.enabled = false;
        polyCollider.enabled = false;
        polyCollider.isTrigger = true;
    }

    public void FireLaser()
    {
        StopAllCoroutines();
        StartCoroutine(LaserRoutine());
    }

    IEnumerator LaserRoutine()
    {
        // 1. 경고 단계 (Warning)
        lineRenderer.enabled = true;

        // (수정) 원뿔 모양 설정
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;

        // 경고 색상
        lineRenderer.startColor = warningColor;
        lineRenderer.endColor = warningColor;

        // (수정) LineRenderer 좌표 설정 (로컬 좌표 기준 아래로)
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, Vector3.zero); // 시작점
        lineRenderer.SetPosition(1, new Vector3(0, -laserLength, 0)); // 끝점

        polyCollider.enabled = false; // 공격 판정 없음

        yield return new WaitForSeconds(warningTime);

        // 2. 발사 단계 (Firing)
        lineRenderer.startColor = fireColor;
        lineRenderer.endColor = fireColor;

        // (수정) PolygonCollider 모양을 레이저 모양(사다리꼴)에 맞춤
        UpdateColliderShape();
        polyCollider.enabled = true; // 공격 판정 활성화

        // (TODO: 사운드)

        yield return new WaitForSeconds(fireDuration);

        // 3. 종료
        lineRenderer.enabled = false;
        polyCollider.enabled = false;
        gameObject.SetActive(false);
    }

    // (신규) Collider 모양을 LineRenderer 모양(사다리꼴)에 맞추는 함수
    void UpdateColliderShape()
    {
        Vector2[] points = new Vector2[4];

        // 상단 (시작점) 좌우
        points[0] = new Vector2(-startWidth / 2, 0);
        points[1] = new Vector2(startWidth / 2, 0);

        // 하단 (끝점) 우좌
        points[2] = new Vector2(endWidth / 2, -laserLength);
        points[3] = new Vector2(-endWidth / 2, -laserLength);

        polyCollider.points = points;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }
}