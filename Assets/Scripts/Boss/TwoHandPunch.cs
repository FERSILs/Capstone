using UnityEngine;
using System.Collections;

public class TwoHandPunch : BossPatternBase
{
    [Header("Pattern Settings")]
    public float delayBetweenHands = 0.8f;     // thời gian nghỉ giữa 2 tay
    public float delayBetweenCycles = 1.5f;    // nghỉ giữa các vòng đấm

    public override void StartPattern(BossController boss)
    {
        boss.StartCoroutine(RunPattern(boss));
    }

    IEnumerator RunPattern(BossController boss)
    {
        while (true) // lặp lại vô hạn để test
        {
            // ✊ Tay trái đấm
            Debug.Log("Tay trái bắt đầu!");
            yield return boss.StartCoroutine(boss.leftHand.LiftHand());
            yield return boss.StartCoroutine(boss.leftHand.SmashHand());

            yield return new WaitForSeconds(delayBetweenHands);

            // ✊ Tay phải đấm
            Debug.Log("Tay phải bắt đầu!");
            yield return boss.StartCoroutine(boss.rightHand.LiftHand());
            yield return boss.StartCoroutine(boss.rightHand.SmashHand());

            yield return new WaitForSeconds(delayBetweenCycles);
        }
    }
}
