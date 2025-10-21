using UnityEngine;
using System;
using PlayerSystem; // PlayerHealth를 참조하기 위해 추가

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    private DialogueData currentData;
    private string[] activeLines;
    private int index;

    // [수정] PlayerHealth를 전달할 수 있는 이벤트로 변경
    public event Action<PlayerHealth> OnDialogueFinished;

    // [추가] 현재 대화 중인 플레이어의 체력 정보
    private PlayerHealth currentPlayerHealth;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // [수정] StartDialogue가 PlayerHealth를 매개변수로 받도록 변경
    public void StartDialogue(DialogueData data, PlayerHealth playerHealth)
    {
        currentData = data;
        activeLines = SelectLinesByCondition(data);
        index = 0;

        // [추가] 플레이어 정보를 저장
        currentPlayerHealth = playerHealth;

        if (activeLines == null || activeLines.Length == 0)
        {
            Debug.LogWarning($"[DialogueManager] No lines for {data?.npcID}");
            EndDialogue();
            return;
        }

        //GameManager.Instance.SetDialogueState(true);
        InputManager.Instance.SetDialogueState(true);
        DialogueUI.Instance.OpenDialogue(data.npcID, activeLines[index]);
    }

    private string[] SelectLinesByCondition(DialogueData data)
    {
        if (data.conditionalDialogues != null)
        {
            foreach (var cd in data.conditionalDialogues)
            {
                if (FlagManager.Instance.CheckFlag(cd.conditionKey))
                    return cd.lines;
            }
        }
        return data.defaultLines;
    }

    private void Update()
    {
        if (DialogueUI.Instance != null && DialogueUI.Instance.IsOpen())
        {
            if (Input.GetKeyDown(KeyCode.Space))
                Next();
        }
    }

    public void Next()
    {
        index++;
        if (index < activeLines.Length)
        {
            DialogueUI.Instance.UpdateDialogue(activeLines[index]);
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        //GameManager.Instance.SetDialogueState(false);
        //GameManager.Instance.BlockInputFor(0.15f);
        InputManager.Instance.SetDialogueState(false); // 수정된 코드
        InputManager.Instance.BlockInputFor(0.15f); // 수정된 코드
        DialogueUI.Instance.CloseDialogue();

        // [수정] 이벤트 호출 시 저장해둔 PlayerHealth 정보를 전달
        OnDialogueFinished?.Invoke(currentPlayerHealth);

        OnDialogueFinished = null;
        currentData = null;
        activeLines = null;
        index = 0;

        // [추가] 플레이어 정보 초기화
        currentPlayerHealth = null;
    }
}