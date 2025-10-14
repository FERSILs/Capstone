using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private HashSet<string> flags = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // 필요하면 여기서 저장 로드(PlayerPrefs/JSON) 연동
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
            Debug.Log($"[GameManager] Flag set: {key}");
        }
    }
}
