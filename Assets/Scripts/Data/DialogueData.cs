using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/NPC Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("NPC 식별용 ID (예: 'Villager01')")]
    public string npcID;

    [System.Serializable]
    public class ConditionalDialogue
    {
        [Tooltip("GameManager 플래그 키 (예: 'Boss1Cleared')")]
        public string conditionKey;

        [TextArea(2, 5)]
        public string[] lines;
    }

    [Header("조건부 대사 목록 (상위 우선)")]
    public ConditionalDialogue[] conditionalDialogues;

    [Header("기본 대사 (조건 불충족 시 사용)")]
    [TextArea(2, 5)]
    public string[] defaultLines;
}
