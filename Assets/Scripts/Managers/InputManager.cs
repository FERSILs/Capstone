// InputManager.cs
using UnityEngine;

/// <summary>
/// 플레이어의 입력 잠금 상태(대화 중, 컷신 등)를 전문적으로 관리합니다.
/// </summary>
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public bool IsInDialogue { get; private set; } = false;
    private float inputBlockedUntil = 0f;

    /// <summary>
    /// 현재 플레이어의 조작이 불가능한 상태인지 확인합니다.
    /// (PlayerController가 이 값을 참조합니다)
    /// </summary>
    public bool IsInputBlocked => IsInDialogue || Time.time < inputBlockedUntil;

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
    /// 대화 상태를 설정합니다. (DialogueManager가 호출)
    /// </summary>
    public void SetDialogueState(bool state)
    {
        IsInDialogue = state;
    }

    /// <summary>
    /// 지정된 시간(초)만큼 입력을 잠급니다. (DialogueManager가 호출)
    /// </summary>
    public void BlockInputFor(float seconds)
    {
        inputBlockedUntil = Mathf.Max(inputBlockedUntil, Time.time + seconds);
    }
}