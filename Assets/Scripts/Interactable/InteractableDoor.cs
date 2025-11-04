// InteractableDoor.cs
using UnityEngine;
using PlayerSystem;
using UnityEngine.SceneManagement; // (중요) 씬 관리를 위해 추가

[RequireComponent(typeof(Collider2D))]
public class InteractableDoor : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public string requiredKeyID = "BossRoomKey";
    public string doorFlag = "Door_BossRoom_Opened";
    [Tooltip("'입장한다'를 선택했을 때 이동할 씬의 이름")]
    public string sceneToLoad = "BossStage_1"; // (이동할 씬 이름)

    [Header("Dialogue")]
    public DialogueData lockedDialogue;
    public DialogueData unlockDialogue;

    [Header("Choice UI References")]
    [Tooltip("1단계에서 만든 선택지 패널 (ChoicePanel)")]
    public GameObject choicePanel;
    [Tooltip("커서 역할을 할 Selector Icon 이미지")]
    public RectTransform selectorIcon;
    [Tooltip("첫 번째 선택지(입장한다)의 Transform")]
    public RectTransform choiceEnterPosition;
    [Tooltip("두 번째 선택지(나간다)의 Transform")]
    public RectTransform choiceLeavePosition;

    private bool isShowingChoices = false;
    private int currentChoice = 0; // 0: 입장한다, 1: 나간다

    // IInteractable 인터페이스 구현
    public void Interact(PlayerInteraction interactor, PlayerHealth health)
    {
        if (isShowingChoices) return; // 이미 선택지가 떠 있으면 무시

        if (FlagManager.Instance.CheckFlag(doorFlag))
        {
            // 이미 열린 문 (바로 입장 로직)
            // (간소화: 이 예제에서는 열린 문은 그냥 통과(비활성화)된 것으로 가정)
            Debug.Log("이미 열린 문입니다.");
            return;
        }

        if (InventoryManager.Instance.HasKey(requiredKeyID))
        {
            // 키가 있음 -> 문 열기 대화 시작
            if (unlockDialogue != null)
            {
                DialogueManager.Instance.StartDialogue(unlockDialogue, health);
                // (중요) 대화가 끝나면 OpenDoor가 아닌 ShowChoices를 호출
                DialogueManager.Instance.OnDialogueFinished += ShowChoices;
            }
            else
            {
                ShowChoices(health); // 대사 없으면 바로 선택지
            }
        }
        else
        {
            // 키가 없음 -> '잠김' 대사 출력
            if (lockedDialogue != null)
            {
                DialogueManager.Instance.StartDialogue(lockedDialogue, health);
            }
        }
    }

    /// <summary>
    /// (신규) 대화가 끝난 후 선택지 UI를 활성화합니다.
    /// </summary>
    private void ShowChoices(PlayerHealth healthContext)
    {
        DialogueManager.Instance.OnDialogueFinished -= ShowChoices;

        if (choicePanel == null)
        {
            Debug.LogError("ChoicePanel이 InteractableDoor에 연결되지 않았습니다!");
            return;
        }

        choicePanel.SetActive(true);
        isShowingChoices = true;
        currentChoice = 0; // 항상 '입장한다'에서 시작
        UpdateSelectorPosition();

        // (중요) InputManager에 '선택 모드'임을 알림
        InputManager.Instance.SetChoiceMenuState(true);

        // (선택 사항) 커서(마우스)를 표시할지 여부
        // Cursor.visible = true;
        // Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// (신규) 선택지 입력을 받기 위한 Update 함수
    /// </summary>
    private void Update()
    {
        if (!isShowingChoices)
        {
            return; // 선택지가 활성화된 상태가 아니면 종료
        }

        // '엔터' 키 입력
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
        {
            HandleChoice();
        }

        // '위' 또는 '아래' 키 입력
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentChoice = (currentChoice + 1) % 2; // 0 -> 1, 1 -> 0
            UpdateSelectorPosition();
        }
    }

    /// <summary>
    /// (신규) 선택지 UI의 커서(SelectorIcon) 위치를 업데이트합니다.
    /// </summary>
    private void UpdateSelectorPosition()
    {
        if (selectorIcon == null) return;

        // (주의) RectTransform의 position을 직접 대입해야 합니다.
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
    /// (신규) '엔터' 키를 눌렀을 때 호출됩니다.
    /// </summary>
    private void HandleChoice()
    {
        // 1. UI 및 상태 비활성화
        isShowingChoices = false;
        choicePanel.SetActive(false);
        InputManager.Instance.SetChoiceMenuState(false);
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;

        // 2. 선택 처리
        if (currentChoice == 0) // "입장한다"
        {
            Debug.Log("'입장한다' 선택!");
            OpenDoorAndLoadScene();
        }
        else // "나중에 간다"
        {
            Debug.Log("'나중에 간다' 선택!");
            // 그냥 닫기 (아무것도 안 함)
        }
    }

    /// <summary>
    /// (수정) 문을 열고 씬을 로드합니다.
    /// </summary>
    private void OpenDoorAndLoadScene()
    {
        Debug.Log($"문 ({requiredKeyID})이 열렸습니다! {sceneToLoad} 씬으로 이동합니다.");

        // 1. 플래그 설정 (다시 잠기지 않도록)
        FlagManager.Instance.SetFlag(doorFlag);

        // 2. 문 오브젝트 비활성화 (선택 사항, 씬이 바뀌면 의미 없음)
        gameObject.SetActive(false);

        // 3. 씬 이동
        // (중요) "File > Build Settings..."에 이동할 씬이 등록되어 있어야 합니다.
        SceneManager.LoadScene(sceneToLoad);
    }
}