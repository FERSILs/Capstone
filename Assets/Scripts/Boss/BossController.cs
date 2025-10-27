using UnityEngine;
using System.Collections.Generic;

public class BossController : MonoBehaviour
{
    [Header("Boss Hands")]
    public BossHand leftHand;
    public BossHand rightHand;

    [Header("Pattern Settings")]
    public List<BossPatternBase> patterns;

    private int currentPatternIndex = 0;

    void Start()
    {
        // Bắt đầu pattern đầu tiên
        if (patterns.Count > 0)
            StartPattern(0);
    }

    public void StartPattern(int index)
    {
        if (index < 0 || index >= patterns.Count)
            return;

        currentPatternIndex = index;
        patterns[index].StartPattern(this);
    }

    public void NextPattern()
    {
        int next = (currentPatternIndex + 1) % patterns.Count;
        StartPattern(next);
    }
}
