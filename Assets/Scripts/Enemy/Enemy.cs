using UnityEngine;
using System.Collections;
using PlayerSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    // 1. FSM ���� ����
    public enum EnemyState
    {
        Idle,
        Alert,
        Chase,
        Return,
        Respawn
    }
    public EnemyState currentState = EnemyState.Idle;

    [Header("Stats & Speeds (3.3)")]
    public float chaseSpeed = 4.2f;
    public float returnSpeed = 3.8f;

    [Header("Rotation")]
    [Tooltip("���� �⺻ ��������Ʈ�� ��(Up)�� ������, ������(Right)�� ������")]
    public bool isFacingUp = true; // (�߿�) 'transform.up'�� �������� ���
    private float startRotation; // (�߰�) ���� �ٶ󺸴� ����

    [Header("Vision Settings (3.2)")]
    public float visionRadius = 3.0f;
    [Range(0, 360)]
    public float visionAngle = 70.0f;
    public float visionCheckInterval = 0.1f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    [Header("Timer Settings (3.3)")]
    public float alertDuration = 0.5f;
    public float chaseLingerTime = 2.0f;
    private float lastTimePlayerSeen;
    private float stateEnterTime;

    [Header("Combat & Respawn (3.1)")]
    public float respawnTime = 4.0f;
    public float contactDamageCooldown = 0.6f;
    private float lastDamageTime = -1f;

    // ������Ʈ �� ��ġ
    private Rigidbody2D rigid;
    private Vector2 startPosition;
    private Transform playerTransform;
    private PlayerHealth playerHealth;
    private SpriteRenderer spriteRenderer;
    private Collider2D physicsCollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.gravityScale = 0f;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;

        physicsCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        startPosition = transform.position;
        startRotation = rigid.rotation; // (�߰�) ���� ���� ����

        if (playerLayer == 0) playerLayer = LayerMask.GetMask("Player");
        if (obstacleLayer == 0) obstacleLayer = LayerMask.GetMask("Walls");
    }

    void Start()
    {
        StartCoroutine(VisionCheckCoroutine());
    }

    // (VisionCheckCoroutine, CheckForPlayer �Լ��� ����... )
    #region Vision Checks
    private IEnumerator VisionCheckCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(visionCheckInterval);
        while (currentState != EnemyState.Respawn)
        {
            yield return wait;
            if (currentState == EnemyState.Idle || currentState == EnemyState.Alert || currentState == EnemyState.Chase)
            {
                bool playerCurrentlyVisible = CheckForPlayer();
                if (playerCurrentlyVisible)
                {
                    lastTimePlayerSeen = Time.time;
                    if (currentState == EnemyState.Idle)
                    {
                        ChangeState(EnemyState.Alert);
                    }
                }
                else
                {
                    if (currentState == EnemyState.Alert)
                    {
                        ChangeState(EnemyState.Idle);
                    }
                }
            }
        }
    }
    private bool CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, visionRadius, playerLayer);
        if (hits.Length == 0) return false;
        Transform target = hits[0].transform;
        Vector2 dirToTarget = (target.position - transform.position).normalized;

        // (����) isFacingUp ������ ���� ���� ���� ���͸� ������
        Vector2 forward = isFacingUp ? (Vector2)transform.up : (Vector2)transform.right;

        if (Vector2.Angle(forward, dirToTarget) < visionAngle / 2)
        {
            float distToTarget = Vector2.Distance(transform.position, target.position);
            RaycastHit2D obstacleHit = Physics2D.Raycast(transform.position, dirToTarget, distToTarget, obstacleLayer);
            if (obstacleHit.collider == null)
            {
                playerTransform = target;
                if (playerHealth == null)
                    playerHealth = target.GetComponent<PlayerHealth>();
                return true;
            }
        }
        return false;
    }
    #endregion

    void FixedUpdate()
    {
        // ������ FSM ȸ�� ���� ��ü ���� ������
        switch (currentState)
        {
            case EnemyState.Idle:
                rigid.linearVelocity = Vector2.zero;
                // (���� ����) õõ�� ���� ������ ���ư��� �� ���� ����
                // rigid.rotation = Mathf.LerpAngle(rigid.rotation, startRotation, Time.fixedDeltaTime * 2f);
                break;

            case EnemyState.Alert:
                rigid.linearVelocity = Vector2.zero; // ���缭
                RotateTowards(playerTransform.position); // �÷��̾ �ٶ�

                if (Time.time - stateEnterTime >= alertDuration)
                {
                    ChangeState(EnemyState.Chase);
                }
                break;

            case EnemyState.Chase:
                if (playerTransform != null)
                {
                    Vector2 dir = (playerTransform.position - transform.position).normalized;
                    rigid.linearVelocity = dir * chaseSpeed;
                    RotateTowards(playerTransform.position); // �̵��ϸ鼭 ��� �ٶ�
                }
                else
                {
                    rigid.linearVelocity = Vector2.zero; // Ÿ�� ���� ������ �ϴ� ����
                }

                if (Time.time - lastTimePlayerSeen > chaseLingerTime)
                {
                    ChangeState(EnemyState.Return);
                }
                break;

            case EnemyState.Return:
                float distance = Vector2.Distance(transform.position, startPosition);

                if (distance > 0.05f)
                {
                    Vector2 dir = (startPosition - (Vector2)transform.position).normalized;
                    rigid.linearVelocity = dir * returnSpeed;
                    RotateTowards(startPosition); // ���� ������ �ٶ󺸸� �̵�
                }
                else
                {
                    // ���� �Ϸ�
                    rigid.linearVelocity = Vector2.zero;
                    rigid.rotation = startRotation; // ���� �ʱ�ȭ
                    ChangeState(EnemyState.Idle);
                }
                break;

            case EnemyState.Respawn:
                rigid.linearVelocity = Vector2.zero;
                break;
        }
    }

    /// <summary>
    /// (�ű� �Լ�) Ư�� ��ġ�� �ٶ󺸵��� Rigidbody�� ������ �����մϴ�.
    /// </summary>
    private void RotateTowards(Vector2 targetPosition)
    {
        Vector2 dir = (targetPosition - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // (�߿�) isFacingUp�� true�̸� 90���� ���� 'transform.up'�� ������ �ǵ��� ����
        if (isFacingUp)
        {
            angle -= 90f;
        }

        // (���� ����) �ε巴�� ȸ��
        rigid.rotation = Mathf.LerpAngle(rigid.rotation, angle, Time.fixedDeltaTime * 10f);
        // (��� ȸ��)
        // rigid.rotation = angle; 
    }

    // (OnCollisionEnter2D, RespawnCoroutine, ChangeState �Լ��� ����...)
    #region Combat & State
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == EnemyState.Respawn) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time - lastDamageTime < contactDamageCooldown)
            {
                return;
            }
            if (playerHealth != null)
            {
                bool damageDealt = playerHealth.TakeDamage(1);
                if (damageDealt)
                {
                    lastDamageTime = Time.time;
                    StartCoroutine(RespawnCoroutine());
                }
            }
        }
    }
    private IEnumerator RespawnCoroutine()
    {
        ChangeState(EnemyState.Respawn);
        physicsCollider.enabled = false;
        spriteRenderer.enabled = false;
        rigid.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(respawnTime);
        transform.position = startPosition;
        rigid.rotation = startRotation; // ���� �ʱ�ȭ
        physicsCollider.enabled = true;
        spriteRenderer.enabled = true;
        ChangeState(EnemyState.Idle);
        StartCoroutine(VisionCheckCoroutine());
    }
    private void ChangeState(EnemyState newState)
    {
        currentState = newState;
        stateEnterTime = Time.time;
    }
    #endregion
}