using UnityEngine;
// using System.Collections.Generic; // HashSet이 필요 없으므로 삭제

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // --- IsInDialogue, IsInputBlocked, flags 등 모든 변수와 함수 삭제 ---
    // (InputManager와 FlagManager로 기능이 이전되었습니다)

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

    // (추후 여기에 LoadScene(), SaveGame(), GameOver() 등이 추가됩니다)
}