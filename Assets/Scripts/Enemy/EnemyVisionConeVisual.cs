using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class EnemyVisionConeVisual : MonoBehaviour
{
    [Header("��ũ (�ʼ�)")]
    public Enemy enemyAI;

    [Header("�ð�ȭ ǰ��")]
    public int rayCount = 30;

    private MeshFilter meshFilter;
    private Mesh mesh;
    private MeshRenderer meshRenderer;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        meshRenderer.sortingLayerName = "Default";
        meshRenderer.sortingOrder = 4;

        meshRenderer.enabled = false;
    }

    void Update()
    {
        if (enemyAI == null) return;

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