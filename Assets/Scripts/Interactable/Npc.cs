using UnityEngine;
using PlayerSystem; // IInteractable 사용

[RequireComponent(typeof(Collider2D))]
public class NPC : MonoBehaviour, IInteractable
{
    public DialogueData dialogueData;

    public void OnInteract()
    {
        if (dialogueData == null)
        {
            Debug.LogWarning($"{name}: DialogueData 미지정");
            return;
        }

        DialogueManager.Instance.StartDialogue(dialogueData);
    }
}
