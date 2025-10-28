using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Boss Hands")]
    public BossHand leftHand;
    public BossHand rightHand;

    [Header("Pattern Settings")]
    public List<BossPatternBase> patterns;

    private int currentPatternIndex = 0;
    private bool isRunningPattern = false;

    void Start()
    {
        if (patterns.Count > 0)
            StartPattern(0);
    }

    public void StartPattern(int index)
    {
        if (index < 0 || index >= patterns.Count) return;

        StopAllCoroutines(); // đảm bảo không chồng pattern
        currentPatternIndex = index;
        isRunningPattern = true;
        patterns[index].StartPattern(this);
    }

    public void NextPattern(float delay = 0f)
    {
        StartCoroutine(StartNextPatternAfterDelay(delay));
    }

    private IEnumerator StartNextPatternAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        int next = (currentPatternIndex + 1) % patterns.Count;
        StartPattern(next);
    }
}
