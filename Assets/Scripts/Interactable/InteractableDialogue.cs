using UnityEngine;
using UnityEngine.Events;
using PlayerSystem;

// 1번 개선안: Npc.cs와 InteractableCorpse.cs를 대체할 통합 스크립트
public class InteractableDialogue : MonoBehaviour, IInteractable
{
    [Header("대화 데이터")]
    [SerializeField] private DialogueData dialogueBefore;
    [SerializeField] private DialogueData dialogueAfter;

    [Header("조건 (선택 사항)")]
    [SerializeField] private string flagToCheck;

    [Header("이벤트 (재사용성)")]
    [Tooltip("플래그가 없을 때, 'Before' 대화가 끝난 직후 딱 한 번 실행됩니다.")]
    public UnityEvent<PlayerHealth> OnFirstDialogueFinished; // PlayerHealth를 받는 이벤트

    private bool hasInteracted = false;

    // [수정] 새 인터페이스 규칙(IInteractable)에 맞게 구현
    public void Interact(PlayerInteraction interactor, PlayerHealth health)
    {
        bool isFlagMet = !string.IsNullOrEmpty(flagToCheck) &&
                          FlagManager.Instance.CheckFlag(flagToCheck);

        if (isFlagMet || (hasInteracted && dialogueAfter != null))
        {
            // [수정] StartDialogue 호출 시 health 전달
            DialogueManager.Instance.StartDialogue(dialogueAfter, health);
        }
        else
        {
            // [수정] StartDialogue 호출 시 health 전달
            DialogueManager.Instance.StartDialogue(dialogueBefore, health);

            // [수정] PlayerHealth를 받는 이벤트(OnDialogueFinished)를 구독
            DialogueManager.Instance.OnDialogueFinished += OnDialogueFinishedCallback;
        }
    }

    // [수정] DialogueManager가 전달해준 PlayerHealth(health)를 받음
    private void OnDialogueFinishedCallback(PlayerHealth health)
    {
        if (hasInteracted) return;

        // [수정] 인스펙터에 연결된 이벤트(예: PlayerHealth.IncreaseMaxHP)를 실행
        OnFirstDialogueFinished?.Invoke(health);

        if (!string.IsNullOrEmpty(flagToCheck))
        {
            FlagManager.Instance.SetFlag(flagToCheck);
        }

        hasInteracted = true;

        // 구독 해제 (중요)
        DialogueManager.Instance.OnDialogueFinished -= OnDialogueFinishedCallback;
    }
}