using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using System; // (추가) System.DateTime.Now를 사용하기 위해

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    private const string saveKeyPrefix = "GameSaveData_";

    public static int slotToLoad = -1;

    // (Awake, OnEnable, OnDisable, OnSceneLoaded 함수는 동일)
    #region Scene Load Management
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (slotToLoad != -1)
        {
            LoadGame(slotToLoad);
            slotToLoad = -1;
        }
    }
    #endregion

    public void SaveGame(int slotIndex)
    {
        Debug.Log($"--- 게임 저장 시작 (Slot {slotIndex}) ---");

        List<string> flags = FlagManager.Instance.GetFlags();
        List<SlotSaveData> inventory = InventoryManager.Instance.GetSaveData();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player 태그를 가진 오브젝트를 찾을 수 없습니다!");
            return;
        }

        // ▼▼▼▼▼ 수정된 부분 ▼▼▼▼▼
        // 현재 시간 문자열 생성
        string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

        GameSaveData saveData = new GameSaveData
        {
            savedFlags = flags,
            savedInventory = inventory,
            playerPosX = player.transform.position.x,
            playerPosY = player.transform.position.y,
            sceneName = SceneManager.GetActiveScene().name,
            saveTime = currentTime // (추가) 시간 저장
        };

        string json = JsonUtility.ToJson(saveData, true);
        string currentSaveKey = saveKeyPrefix + slotIndex;
        PlayerPrefs.SetString(currentSaveKey, json);
        PlayerPrefs.Save();

        Debug.Log($"저장 완료: {currentSaveKey}");
    }

    public void LoadGame(int slotIndex)
    {
        // (LoadGame 함수는 수정 없음, OnSceneLoaded가 호출)
        Debug.Log($"--- 게임 로드 시작 (Slot {slotIndex}) ---");

        string currentSaveKey = saveKeyPrefix + slotIndex;
        string json = PlayerPrefs.GetString(currentSaveKey, null);

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning($"저장된 데이터가 없습니다. (Slot {slotIndex})");
            return;
        }

        GameSaveData loadData = JsonUtility.FromJson<GameSaveData>(json);

        FlagManager.Instance.LoadFlags(loadData.savedFlags);
        InventoryManager.Instance.LoadSaveData(loadData.savedInventory);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(loadData.playerPosX, loadData.playerPosY, player.transform.position.z);
        }

        Debug.Log("--- 게임 로드 완료 ---");
    }

    // ▼▼▼▼▼ 추가된 함수 ▼▼▼▼▼
    /// <summary>
    /// (PauseMenu용) 저장 파일을 '로드'하지 않고,
    /// 씬 이름과 시간 같은 '정보'만 미리 읽어옵니다.
    /// </summary>
    public GameSaveData GetSaveDataInfo(int slotIndex)
    {
        string currentSaveKey = saveKeyPrefix + slotIndex;
        string json = PlayerPrefs.GetString(currentSaveKey, null);

        if (string.IsNullOrEmpty(json))
        {
            return null; // 빈 슬롯
        }

        GameSaveData info = JsonUtility.FromJson<GameSaveData>(json);
        return info;
    }
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲
}

// (수정) GameSaveData에 saveTime 변수 추가
[System.Serializable]
public class GameSaveData
{
    public List<string> savedFlags;
    public List<SlotSaveData> savedInventory;
    public float playerPosX;
    public float playerPosY;
    public string sceneName;
    public string saveTime; // (추가)
}

// (SlotSaveData 클래스는 동일)
[System.Serializable]
public class SlotSaveData
{
    public string itemName;
    public int amount;
}