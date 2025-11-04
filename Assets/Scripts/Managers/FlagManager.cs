// FlagManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq; // (List 변환을 위해 추가)

public class FlagManager : MonoBehaviour
{
    public static FlagManager Instance;

    // (수정) private -> public (DataManager가 접근해야 함)
    public HashSet<string> flags = new HashSet<string>();

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

    public bool CheckFlag(string key) => flags.Contains(key);

    public void SetFlag(string key)
    {
        if (flags.Add(key))
        {
            Debug.Log($"[FlagManager] Flag set: {key}");
        }
    }

    // ▼▼▼▼▼ 추가된 함수 ▼▼▼▼▼

    /// <summary>
    /// (DataManager용) 저장할 플래그 리스트를 반환합니다.
    /// </summary>
    public List<string> GetFlags()
    {
        return flags.ToList(); // HashSet을 List로 변환하여 반환
    }

    /// <summary>
    /// (DataManager용) 로드된 플래그 리스트로 덮어씁니다.
    /// </summary>
    public void LoadFlags(List<string> loadedFlags)
    {
        flags = new HashSet<string>(loadedFlags);
        Debug.Log($"[FlagManager] 플래그 {flags.Count}개 로드됨.");
    }
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲
}