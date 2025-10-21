using System.Collections;
using UnityEngine;

namespace PlayerSystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody2D rigid;
        private Vector2 inputVec;
        private Vector2 lastMoveDir = Vector2.down;
        public Vector2 LastMoveDir => lastMoveDir;
        private Vector2 rollDirection;

        [Header("Movement Settings")]
        public float moveSpeed = 5f;

        [Header("Roll Settings")]
        public float rollSpeed = 8f;
        public float rollDuration = 1.3f;
        public float rollCooldown = 1f;
        public float invincibleTime = 0.2f;

        private bool isRolling = false;
        private bool canRoll = true;
        public bool isInvincible { get; private set; } = false;

        private void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            rigid.gravityScale = 0f;
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        private void Update()
        {
            //if (GameManager.Instance != null && GameManager.Instance.IsInputBlocked)
            if (InputManager.Instance != null && InputManager.Instance.IsInputBlocked) // 수정된 코드
            {
                rigid.linearVelocity = Vector2.zero;
                return;
            }
            inputVec.x = Input.GetAxisRaw("Horizontal");
            inputVec.y = Input.GetAxisRaw("Vertical");

            if (inputVec != Vector2.zero)
                lastMoveDir = inputVec.normalized;

            if (Input.GetKeyDown(KeyCode.Space) && canRoll && inputVec != Vector2.zero)
                StartCoroutine(Roll());
        }

        private void FixedUpdate()
        {
            if (isRolling)
                rigid.linearVelocity = rollDirection * rollSpeed;
            else
                rigid.linearVelocity = inputVec.normalized * moveSpeed;
        }

        private IEnumerator Roll()
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
    }
}
