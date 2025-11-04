using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // (추가) TextMeshPro를 사용하기 위해
using System.Collections.Generic; // (List 사용 시 필요할 수 있음)

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    [Header("UI Panels")]
    public GameObject pauseMenuPanel;
    public GameObject saveSlotPanel;
    public GameObject loadSlotPanel;

    // ▼▼▼▼▼ 추가된 부분 ▼▼▼▼▼
    [Header("Slot Display Texts (Size = 3)")]
    [Tooltip("SaveSlotPanel의 씬 이름 텍스트 3개")]
    public TextMeshProUGUI[] saveSlotSceneTexts;
    [Tooltip("SaveSlotPanel의 시간 텍스트 3개")]
    public TextMeshProUGUI[] saveSlotTimeTexts;
    [Tooltip("LoadSlotPanel의 씬 이름 텍스트 3개")]
    public TextMeshProUGUI[] loadSlotSceneTexts;
    [Tooltip("LoadSlotPanel의 시간 텍스트 3개")]
    public TextMeshProUGUI[] loadSlotTimeTexts;
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

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

    // (1) 메인 메뉴 버튼
    public void OnContinueButton()
    {
        TogglePauseMenu();
    }

    public void OnOpenSavePanelButton()
    {
        // ▼▼▼▼▼ 수정된 부분 ▼▼▼▼▼
        UpdateSlotDisplay(); // (추가) 패널 열기 전 텍스트 갱신
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲
        pauseMenuPanel.SetActive(false);
        saveSlotPanel.SetActive(true);
    }

    public void OnOpenLoadPanelButton()
    {
        // ▼▼▼▼▼ 수정된 부분 ▼▼▼▼▼
        UpdateSlotDisplay(); // (추가) 패널 열기 전 텍스트 갱신
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲
        pauseMenuPanel.SetActive(false);
        loadSlotPanel.SetActive(true);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    // (2) 저장/로드 슬롯 패널 버튼

    public void OnSaveSlotClicked(int slotIndex)
    {
        DataManager.Instance.SaveGame(slotIndex);
        Debug.Log($"Slot {slotIndex}에 저장 완료.");
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

    // ▼▼▼▼▼ 추가된 함수 ▼▼▼▼▼
    /// <summary>
    /// 모든 저장/로드 슬롯의 UI 텍스트를 갱신합니다.
    /// </summary>
    private void UpdateSlotDisplay()
    {
        // 3개의 슬롯 (0, 1, 2)을 순회
        for (int i = 0; i < 3; i++)
        {
            GameSaveData info = DataManager.Instance.GetSaveDataInfo(i);

            if (info != null)
            {
                // 저장 데이터가 있음
                if (saveSlotSceneTexts[i] != null) saveSlotSceneTexts[i].text = info.sceneName;
                if (saveSlotTimeTexts[i] != null) saveSlotTimeTexts[i].text = info.saveTime;

                if (loadSlotSceneTexts[i] != null) loadSlotSceneTexts[i].text = info.sceneName;
                if (loadSlotTimeTexts[i] != null) loadSlotTimeTexts[i].text = info.saveTime;
            }
            else
            {
                // 빈 슬롯
                if (saveSlotSceneTexts[i] != null) saveSlotSceneTexts[i].text = "Empty Slot";
                if (saveSlotTimeTexts[i] != null) saveSlotTimeTexts[i].text = "--:--";

                if (loadSlotSceneTexts[i] != null) loadSlotSceneTexts[i].text = "Empty Slot";
                if (loadSlotTimeTexts[i] != null) loadSlotTimeTexts[i].text = "--:--";
            }
        }
    }
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲
}