// InteractableDoor.cs
using UnityEngine;
using PlayerSystem;
using UnityEngine.SceneManagement; // (�߿�) �� ������ ���� �߰�

[RequireComponent(typeof(Collider2D))]
public class InteractableDoor : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public string requiredKeyID = "BossRoomKey";
    public string doorFlag = "Door_BossRoom_Opened";
    [Tooltip("'�����Ѵ�'�� �������� �� �̵��� ���� �̸�")]
    public string sceneToLoad = "BossStage_1"; // (�̵��� �� �̸�)

    [Header("Dialogue")]
    public DialogueData lockedDialogue;
    public DialogueData unlockDialogue;

    [Header("Choice UI References")]
    [Tooltip("1�ܰ迡�� ���� ������ �г� (ChoicePanel)")]
    public GameObject choicePanel;
    [Tooltip("Ŀ�� ������ �� Selector Icon �̹���")]
    public RectTransform selectorIcon;
    [Tooltip("ù ��° ������(�����Ѵ�)�� Transform")]
    public RectTransform choiceEnterPosition;
    [Tooltip("�� ��° ������(������)�� Transform")]
    public RectTransform choiceLeavePosition;

    private bool isShowingChoices = false;
    private int currentChoice = 0; // 0: �����Ѵ�, 1: ������

    // IInteractable �������̽� ����
    public void Interact(PlayerInteraction interactor, PlayerHealth health)
    {
        if (isShowingChoices) return; // �̹� �������� �� ������ ����

        if (FlagManager.Instance.CheckFlag(doorFlag))
        {
            // �̹� ���� �� (�ٷ� ���� ����)
            // (����ȭ: �� ���������� ���� ���� �׳� ���(��Ȱ��ȭ)�� ������ ����)
            Debug.Log("�̹� ���� ���Դϴ�.");
            return;
        }

        if (InventoryManager.Instance.HasKey(requiredKeyID))
        {
            // Ű�� ���� -> �� ���� ��ȭ ����
            if (unlockDialogue != null)
            {
                DialogueManager.Instance.StartDialogue(unlockDialogue, health);
                // (�߿�) ��ȭ�� ������ OpenDoor�� �ƴ� ShowChoices�� ȣ��
                DialogueManager.Instance.OnDialogueFinished += ShowChoices;
            }
            else
            {
                ShowChoices(health); // ��� ������ �ٷ� ������
            }
        }
        else
        {
            // Ű�� ���� -> '���' ��� ���
            if (lockedDialogue != null)
            {
                DialogueManager.Instance.StartDialogue(lockedDialogue, health);
            }
        }
    }

    /// <summary>
    /// (�ű�) ��ȭ�� ���� �� ������ UI�� Ȱ��ȭ�մϴ�.
    /// </summary>
    private void ShowChoices(PlayerHealth healthContext)
    {
        DialogueManager.Instance.OnDialogueFinished -= ShowChoices;

        if (choicePanel == null)
        {
            Debug.LogError("ChoicePanel�� InteractableDoor�� ������� �ʾҽ��ϴ�!");
            return;
        }

        choicePanel.SetActive(true);
        isShowingChoices = true;
        currentChoice = 0; 
        UpdateSelectorPosition();

        InputManager.Instance.SetChoiceMenuState(true);

    }
    private void Update()
    {
        if (!isShowingChoices)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
        {
            HandleChoice();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentChoice = (currentChoice + 1) % 2; // 0 -> 1, 1 -> 0
            UpdateSelectorPosition();
        }
    }

    private void UpdateSelectorPosition()
    {
        if (selectorIcon == null) return;

        if (currentChoice == 0)
        {
            selectorIcon.position = choiceEnterPosition.position;
        }
        else
        {
            selectorIcon.position = choiceLeavePosition.position;
        }
    }

    /// <summary>
    /// (�ű�) '����' Ű�� ������ �� ȣ��˴ϴ�.
    /// </summary>
    private void HandleChoice()
    {
        // 1. UI �� ���� ��Ȱ��ȭ
        isShowingChoices = false;
        choicePanel.SetActive(false);
        InputManager.Instance.SetChoiceMenuState(false);
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;

        // 2. ���� ó��
        if (currentChoice == 0) // "�����Ѵ�"
        {
            Debug.Log("'�����Ѵ�' ����!");
            OpenDoorAndLoadScene();
        }
        else // "���߿� ����"
        {
            Debug.Log("'���߿� ����' ����!");
            // �׳� �ݱ� (�ƹ��͵� �� ��)
        }
    }

    /// <summary>
    /// (����) ���� ���� ���� �ε��մϴ�.
    /// </summary>
    private void OpenDoorAndLoadScene()
    {
        Debug.Log($"�� ({requiredKeyID})�� ���Ƚ��ϴ�! {sceneToLoad} ������ �̵��մϴ�.");

        // 1. �÷��� ���� (�ٽ� ����� �ʵ���)
        FlagManager.Instance.SetFlag(doorFlag);

        // 2. �� ������Ʈ ��Ȱ��ȭ (���� ����, ���� �ٲ�� �ǹ� ����)
        gameObject.SetActive(false);

        // 3. �� �̵�
        // (�߿�) "File > Build Settings..."�� �̵��� ���� ��ϵǾ� �־�� �մϴ�.
        SceneManager.LoadScene(sceneToLoad);
    }
}