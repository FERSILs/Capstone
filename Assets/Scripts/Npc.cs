using UnityEngine;
using static player;

public class NPC : MonoBehaviour, IInteractable
{
    public void OnInteract()
    {
        Debug.Log($"{name}: 대화창 열기!");
    }
}
