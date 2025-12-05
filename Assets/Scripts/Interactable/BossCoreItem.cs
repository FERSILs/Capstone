using UnityEngine;
using PlayerSystem; // IPickup 사용

[RequireComponent(typeof(Collider2D))]
public class BossCoreItem : MonoBehaviour, IPickup
{
    // (나중에 BossController나 GameManager와 연동하여 카운트를 올립니다)
    // 지금은 획득 로그만 출력하고 사라집니다.

    public void Pickup(PlayerInteraction interactor)
    {
        Debug.Log(">>> 보스 코어 아이템 획득! (카운트 +1)");

        // TODO: BossController.Instance.AddCoreCount(); 같은 로직 추가 예정

        Destroy(gameObject);
    }
}