using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // (�߰�) TextMeshPro�� ����ϱ� ����
using System.Collections.Generic; // (List ��� �� �ʿ��� �� ����)

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    [Header("UI Panels")]
    public GameObject pauseMenuPanel;
    public GameObject saveSlotPanel;
    public GameObject loadSlotPanel;

    // ������ �߰��� �κ� ������
    [Header("Slot Display Texts (Size = 3)")]
    [Tooltip("SaveSlotPanel�� �� �̸� �ؽ�Ʈ 3��")]
    public TextMeshProUGUI[] saveSlotSceneTexts;
    [Tooltip("SaveSlotPanel�� �ð� �ؽ�Ʈ 3��")]
    public TextMeshProUGUI[] saveSlotTimeTexts;
    [Tooltip("LoadSlotPanel�� �� �̸� �ؽ�Ʈ 3��")]
    public TextMeshProUGUI[] loadSlotSceneTexts;
    [Tooltip("LoadSlotPanel�� �ð� �ؽ�Ʈ 3��")]
    public TextMeshProUGUI[] loadSlotTimeTexts;
    // ������������������

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        pauseMenuPanel.SetActive(false);
        saveSlotPanel.SetActive(false);
        loadSlotPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (InputManager.Instance.IsInDialogue || InputManager.Instance.IsInChoiceMenu)
            {
                return;
            }

            if (InputManager.Instance.IsInInventory)
            {
                InventoryUI.Instance.Toggle();
                return;
            }

            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        isPaused = !isPaused;
        pauseMenuPanel.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f;
            InputManager.Instance.SetPauseMenuState(true);
        }
        else
        {
            Time.timeScale = 1f;
            InputManager.Instance.SetPauseMenuState(false);

            saveSlotPanel.SetActive(false);
            loadSlotPanel.SetActive(false);
        }
    }

    // --- (UI Button OnClick() Events) ---

    // (1) ���� �޴� ��ư
    public void OnContinueButton()
    {
        TogglePauseMenu();
    }

    public void OnOpenSavePanelButton()
    {
        // ������ ������ �κ� ������
        UpdateSlotDisplay(); // (�߰�) �г� ���� �� �ؽ�Ʈ ����
        // ������������������
        pauseMenuPanel.SetActive(false);
        saveSlotPanel.SetActive(true);
    }

    public void OnOpenLoadPanelButton()
    {
        // ������ ������ �κ� ������
        UpdateSlotDisplay(); // (�߰�) �г� ���� �� �ؽ�Ʈ ����
        // ������������������
        pauseMenuPanel.SetActive(false);
        loadSlotPanel.SetActive(true);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    // (2) ����/�ε� ���� �г� ��ư

    public void OnSaveSlotClicked(int slotIndex)
    {
        DataManager.Instance.SaveGame(slotIndex);
        Debug.Log($"Slot {slotIndex}�� ���� �Ϸ�.");
        TogglePauseMenu();
    }

    public void OnLoadSlotClicked(int slotIndex)
    {
        DataManager.slotToLoad = slotIndex;
        Time.timeScale = 1f;

        InputManager.Instance.ResetAllInputStates();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnBackButton()
    {
        saveSlotPanel.SetActive(false);
        loadSlotPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    // ������ �߰��� �Լ� ������
    /// <summary>
    /// ��� ����/�ε� ������ UI �ؽ�Ʈ�� �����մϴ�.
    /// </summary>
    private void UpdateSlotDisplay()
    {
        // 3���� ���� (0, 1, 2)�� ��ȸ
        for (int i = 0; i < 3; i++)
        {
            GameSaveData info = DataManager.Instance.GetSaveDataInfo(i);

            if (info != null)
            {
                // ���� �����Ͱ� ����
                if (saveSlotSceneTexts[i] != null) saveSlotSceneTexts[i].text = info.sceneName;
                if (saveSlotTimeTexts[i] != null) saveSlotTimeTexts[i].text = info.saveTime;

                if (loadSlotSceneTexts[i] != null) loadSlotSceneTexts[i].text = info.sceneName;
                if (loadSlotTimeTexts[i] != null) loadSlotTimeTexts[i].text = info.saveTime;
            }
            else
            {
                // �� ����
                if (saveSlotSceneTexts[i] != null) saveSlotSceneTexts[i].text = "Empty Slot";
                if (saveSlotTimeTexts[i] != null) saveSlotTimeTexts[i].text = "--:--";

                if (loadSlotSceneTexts[i] != null) loadSlotSceneTexts[i].text = "Empty Slot";
                if (loadSlotTimeTexts[i] != null) loadSlotTimeTexts[i].text = "--:--";
            }
        }
    }
    // ������������������
}