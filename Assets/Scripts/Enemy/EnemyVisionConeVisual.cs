using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class EnemyVisionConeVisual : MonoBehaviour
{
    [Header("링크 (필수)")]
    public Enemy enemyAI; // 부모 오브젝트의 Enemy.cs

    [Header("시각화 품질")]
    public int rayCount = 30; // 원뿔을 몇 개의 삼각형으로 그릴지

    private MeshFilter meshFilter;
    private Mesh mesh;
    private MeshRenderer meshRenderer;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        // ▼▼▼▼▼ 수정된 부분 ▼▼▼▼▼
        // MeshRenderer의 그리기 순서를 스크립트로 직접 설정합니다.
        meshRenderer.sortingLayerName = "Default"; // 땅, 플레이어와 동일한 레이어
        meshRenderer.sortingOrder = 4; // 땅(0)보다는 위, 몬스터(5)보다는 아래
                                       // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

        // 처음엔 숨김
        meshRenderer.enabled = false;
    }

    void Update()
    {
        if (enemyAI == null) return;

        // 'Chase' 또는 'Alert' 상태일 때만 시야각 표시
        if (enemyAI.currentState == Enemy.EnemyState.Chase || enemyAI.currentState == Enemy.EnemyState.Alert)
        {
            if (!meshRenderer.enabled) meshRenderer.enabled = true;
            DrawVisionCone();
        }
        else
        {
            if (meshRenderer.enabled) meshRenderer.enabled = false;
        }
    }

    // DrawVisionCone 함수는 동일 (생략)
    void DrawVisionCone()
    {
        float fov = enemyAI.visionAngle;
        float radius = enemyAI.visionRadius;
        LayerMask blockLayer = enemyAI.obstacleLayer;

        float startAngle = -fov / 2;
        float angleStep = fov / (rayCount - 1);

        Vector3[] vertices = new Vector3[rayCount + 1];
        int[] triangles = new int[(rayCount - 1) * 3];

        vertices[0] = Vector3.zero;

        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector3 dir = Quaternion.Euler(0, 0, angle) * transform.up;

            Vector3 vertexPosition;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, radius, blockLayer);

            if (hit.collider == null)
            {
                vertexPosition = transform.position + dir * radius;
            }
            else
            {
                vertexPosition = hit.point;
            }

            vertices[vertexIndex] = transform.InverseTransformPoint(vertexPosition);

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }
            vertexIndex++;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}