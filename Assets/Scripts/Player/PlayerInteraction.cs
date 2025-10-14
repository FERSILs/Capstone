using UnityEngine;

namespace PlayerSystem
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Interaction Settings")]
        public float interactRange = 1.5f;   // 정면 Ray 거리
        public float pickupRange = 1.2f;     // 주변 탐색 거리

        private PlayerController controller;

        // 디버그 표시용 변수
        private Vector2 lastRayDir = Vector2.zero;
        private bool hitSomething = false;
        private string hitName = "";

        private void Awake()
        {
            controller = GetComponent<PlayerController>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
                TryInteract();
        }

        private void TryInteract()
        {
            Vector2 pos = transform.position;
            Vector2 dir = controller.LastMoveDir;
            lastRayDir = dir;

            hitSomething = false;
            hitName = "";

            Debug.Log($"[TryInteract] Ray 방향: {dir}, 거리: {interactRange}");

            //  정면 Raycast (NPC, 문 등)
            RaycastHit2D hitFront = Physics2D.Raycast(pos, dir, interactRange, LayerMask.GetMask("Interactable"));
            if (hitFront.collider != null)
            {
                hitSomething = true;
                hitName = hitFront.collider.name;

                Debug.Log($"정면 감지됨: {hitFront.collider.name}");

                var interactable = hitFront.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.OnInteract();
                    return; //  상호작용 성공 → 함수 종료
                }

                return; //  감지는 됐지만 인터페이스가 없을 때도 종료
            }

            // 2 주변 아이템 탐색 (Pickup Layer)
            Collider2D[] hitAround = Physics2D.OverlapCircleAll(pos, pickupRange, LayerMask.GetMask("Pickup"));
            foreach (var col in hitAround)
            {
                hitSomething = true;
                hitName = col.name;

                var pickup = col.GetComponent<IPickup>();
                if (pickup != null)
                {
                    pickup.OnPickup();
                    return; //  아이템 획득 후 종료
                }
            }

            //  아무 상호작용 대상도 없을 때만 로그 출력
            if (!hitSomething)
            {
                Debug.Log("상호작용 가능한 오브젝트 없음");
            }
        }

        //  Scene 뷰 시각화
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || controller == null)
                return;

            // Ray 색상 (빨강 = 감지 X / 초록 = 감지 O)
            Gizmos.color = hitSomething ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, lastRayDir * interactRange);

            // Pickup 감지 원 (노랑 투명)
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, pickupRange);

#if UNITY_EDITOR
            // Scene 뷰에 감지된 오브젝트 이름 표시
            if (hitSomething && !string.IsNullOrEmpty(hitName))
            {
                UnityEditor.Handles.Label(
                    transform.position + (Vector3)lastRayDir * interactRange,
                    $"Hit: {hitName}"
                );
            }
#endif
        }
    }
}
