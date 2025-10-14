using UnityEngine;
using PlayerSystem;

public class Item : MonoBehaviour, IPickup
{
    public void OnPickup()
    {
        Debug.Log($"{name}: 아이템 획득!");
        Destroy(gameObject);
    }
}
