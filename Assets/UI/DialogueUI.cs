using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;  // 싱글톤 접근용

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI dialogueText;

    private bool isActive = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Canvas 자체는 켜두고, 내부 Panel만 꺼서 시작
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }


    // 대화창 열기
    public void OpenDialogue(string npcName, string firstLine)
    {
        dialoguePanel.SetActive(true);
        npcNameText.text = npcName;
        dialogueText.text = firstLine;
        isActive = true;
    }

    // 다음 문장 표시
    public void UpdateDialogue(string line)
    {
        dialogueText.text = line;
    }

    // 닫기
    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        isActive = false;
    }

    public bool IsOpen() => isActive;
}
