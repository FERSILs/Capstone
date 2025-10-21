// FlagManager.cs
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 게임의 모든 플래그(이벤트 완료 여부)를 저장하고 관리합니다.
/// </summary>
public class FlagManager : MonoBehaviour
{
    public static FlagManager Instance;

    private HashSet<string> flags = new HashSet<string>();

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

    /// <summary>
    /// 해당 플래그가 저장되었는지 확인합니다.
    /// </summary>
    public bool CheckFlag(string key) => flags.Contains(key);

    /// <summary>
    /// 새로운 플래그를 저장합니다.
    /// </summary>
    public void SetFlag(string key)
    {
        if (flags.Add(key))
        {
            Debug.Log($"[FlagManager] Flag set: {key}");
        }
    }

    // (참고: 추후 여기에 SaveFlags(), LoadFlags() 함수를 만들면 됩니다.)
}