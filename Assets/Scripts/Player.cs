using System.Collections;
using UnityEngine;

public class player : MonoBehaviour
{
    Rigidbody2D rigid;
    private Vector2 lastMoveDir = Vector2.down; // 마지막으로 이동한 방향 (기본 아래)

    public float player_speed = 5f;     // 일반 이동 속도

    public float rollSpeed = 8f;        // 구르기 속도
    public float rollDuration = 0.3f;   // 구르기 지속 시간
    public float rollCooldown = 1f;     // 구르기 쿨타임
    public float invincibleTime = 0.2f; // 구르기 무적 시간

    private Vector2 inputVec;            // 입력 벡터
    private Vector2 rollDirection;       // 구르기 방향
    private bool isRolling = false;      // 구르기 중인지
    private bool canRoll = true;         // 구르기 가능 여부
    public bool isInvincible = false;    // 무적 상태


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        rigid.gravityScale = 0f;                            
        rigid.angularVelocity = 0f;                          
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation; // 회전 고정
    }

    void Update()
    {
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");

        // 이동 방향이 0이 아닐 때만 lastMoveDir 업데이트
        if (inputVec != Vector2.zero)
            lastMoveDir = inputVec.normalized;

        if (Input.GetKeyDown(KeyCode.Space) && canRoll && inputVec != Vector2.zero)
        {
            StartCoroutine(Roll());
        }

        if (Input.GetKeyDown(KeyCode.F)) // 상호작용 키 (예: F)
            TryInteract();
    }

    void FixedUpdate()
    {
        if (isRolling)
        {
            rigid.linearVelocity = rollDirection * rollSpeed;
        }
        else
        {
            rigid.linearVelocity = inputVec.normalized * player_speed;
        }
    }
    void TryInteract()
    {
        // 정면 상호작용 (앞으로 쏘는 Ray)
        RaycastHit2D hitFront = Physics2D.Raycast(transform.position, lastMoveDir, 1f, LayerMask.GetMask("Interactable"));
        if (hitFront.collider != null)
        {
            Debug.Log("정면 오브젝트 감지: " + hitFront.collider.name);

            IInteractable interactable = hitFront.collider.GetComponent<IInteractable>();
            if (interactable != null)
                interactable.OnInteract();
            return; // 정면 상호작용 우선
        }

        // 주변 상호작용 (360도 반경 탐색)
        Collider2D[] hitAround = Physics2D.OverlapCircleAll(transform.position, 1.2f, LayerMask.GetMask("Pickup"));
        foreach (var col in hitAround)
        {
            IPickup pickup = col.GetComponent<IPickup>();
            if (pickup != null)
            {
                pickup.OnPickup();
                return;
            }
        }

        Debug.Log("상호작용 가능한 오브젝트 없음");
    }

    public interface IInteractable
    {
        void OnInteract(); // NPC, 문 등
    }

    public interface IPickup
    {
        void OnPickup();   // 아이템 등
    }


    IEnumerator Roll()
    {
        canRoll = false;
        isRolling = true;
        rollDirection = inputVec.normalized;

        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false; 

        yield return new WaitForSeconds(rollDuration - invincibleTime);

        isRolling = false;

        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible)
        {
            Debug.Log("무적 상태로 인해 데미지 무시!");
            return;
        }

        Debug.Log("데미지 받음: " + damage);
    }
}
