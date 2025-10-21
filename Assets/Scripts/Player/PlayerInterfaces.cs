using UnityEngine;

namespace PlayerSystem
{
    // 상호작용 가능한 객체 (NPC, 문 등)
    public interface IInteractable
    {
        // [수정] PlayerHealth 컴포넌트도 함께 전달하도록 변경
        void Interact(PlayerInteraction interactor, PlayerHealth health);
    }

    // 플레이어가 습득 가능한 오브젝트 (아이템 등)
    public interface IPickup
    {
        // (참고: IPickup도 필요하다면 동일하게 수정할 수 있습니다)
        void Pickup(PlayerInteraction interactor);
    }
}