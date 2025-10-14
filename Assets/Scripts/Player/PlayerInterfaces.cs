using UnityEngine;

namespace PlayerSystem
{
    // 상호작용 가능한 객체 (NPC, 문 등)
    public interface IInteractable
    {
        void OnInteract();
    }

    // 플레이어가 습득 가능한 오브젝트 (아이템 등)
    public interface IPickup
    {
        void OnPickup();
    }
}
