// InputManager.cs
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public bool IsInDialogue { get; private set; } = false;
    private float inputBlockedUntil = 0f;
    public bool IsInChoiceMenu { get; private set; } = false;
    public bool IsInPauseMenu { get; private set; } = false;

    // ▼▼▼▼▼ 추가된 부분 ▼▼▼▼▼
    [Header("Inventory State")]
    public bool IsInInventory { get; private set; } = false;
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

    public bool IsInputBlocked => IsInDialogue || Time.time < inputBlockedUntil || IsInChoiceMenu || IsInPauseMenu || IsInInventory; // (수정) IsInInventory 추가

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
    // (SetDialogueState, BlockInputFor, SetChoiceMenuState, SetPauseMenuState 함수는 동일)
    public void SetDialogueState(bool state) { IsInDialogue = state; }
    public void BlockInputFor(float seconds) { inputBlockedUntil = Mathf.Max(inputBlockedUntil, Time.time + seconds); }
    public void SetChoiceMenuState(bool state) { IsInChoiceMenu = state; }
    public void SetPauseMenuState(bool state) { IsInPauseMenu = state; }

    public void SetInventoryState(bool state)
    {
        IsInInventory = state;
    }
    public void ResetAllInputStates()
    {
        IsInDialogue = false;
        IsInChoiceMenu = false;
        IsInPauseMenu = false;
        IsInInventory = false;
    }
}