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

        // ▼▼▼▼▼ 추가된 변수 ▼▼▼▼▼
        private float rollStartTime;  // 구르기 시작 시간
        private float totalRollTime;  // (구르기 + 쿨타임) 총 시간
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

        private void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            rigid.gravityScale = 0f;
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;

            // ▼▼▼▼▼ 추가된 줄 ▼▼▼▼▼
            // 구르기 지속시간 + 쿨타임을 합친 총 시간을 미리 계산
            totalRollTime = rollDuration + rollCooldown;
            // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲
        }

        private void Update()
        {
            // (입력 처리)
            if (InputManager.Instance != null && InputManager.Instance.IsInputBlocked)
            {
                rigid.linearVelocity = Vector2.zero;
                inputVec = Vector2.zero;
                UpdateRollUI(); // (추가) 입력이 잠겨도 UI는 갱신
                return;
            }
            inputVec.x = Input.GetAxisRaw("Horizontal");
            inputVec.y = Input.GetAxisRaw("Vertical");

            if (inputVec != Vector2.zero)
                lastMoveDir = inputVec.normalized;

            if (Input.GetKeyDown(KeyCode.Space) && canRoll && inputVec != Vector2.zero)
            {
                // (수정) 코루틴 시작과 동시에 시간 기록
                rollStartTime = Time.time;
                StartCoroutine(Roll());
            }

            // (추가) 구르기 쿨타임 UI 갱신 로직
            UpdateRollUI();
        }

        // ▼▼▼▼▼ 추가된 함수 ▼▼▼▼▼
        /// <summary>
        /// 구르기 쿨타임 UI를 매 프레임 갱신합니다.
        /// </summary>
        private void UpdateRollUI()
        {
            if (PlayerHUD.Instance == null) return;

            if (canRoll)
            {
                // 구르기 가능: 쿨타임 바 100%
                PlayerHUD.Instance.UpdateRollCooldown(1f);
            }
            else
            {
                // 구르기/쿨타임 진행 중: (현재시간 - 시작시간) / 총 시간
                float progress = (Time.time - rollStartTime) / totalRollTime;
                PlayerHUD.Instance.UpdateRollCooldown(Mathf.Clamp01(progress));
            }
        }
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

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