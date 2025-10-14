using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    private DialogueData currentData;
    private string[] activeLines;
    private int index;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartDialogue(DialogueData data)
    {
        currentData = data;
        activeLines = SelectLinesByCondition(data);
        index = 0;

        if (activeLines == null || activeLines.Length == 0)
        {
            Debug.LogWarning($"[DialogueManager] No lines for {data?.npcID}");
            EndDialogue();
            return;
        }

        // UI 열기
        DialogueUI.Instance.OpenDialogue(data.npcID, activeLines[index]);
    }

    private string[] SelectLinesByCondition(DialogueData data)
    {
        if (data.conditionalDialogues != null)
        {
            foreach (var cd in data.conditionalDialogues)
            {
                if (GameManager.Instance.CheckFlag(cd.conditionKey))
                    return cd.lines;
            }
        }
        return data.defaultLines;
    }

    private void Update()
    {
        // Space 키로 다음 문장 넘기기
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
        DialogueUI.Instance.CloseDialogue();
        currentData = null;
        activeLines = null;
        index = 0;
    }
}
