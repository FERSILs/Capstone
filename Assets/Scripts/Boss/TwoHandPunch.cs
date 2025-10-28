using UnityEngine;
using System.Collections;

public class TwoHandPunch : BossPatternBase
{
    [Header("Pattern Settings")]
    public float delayBetweenHands = 0.8f;
    public float delayBetweenCycles = 1.5f;

    public override void StartPattern(BossController boss)
    {
        if (boss.leftHand == null || boss.rightHand == null)
        {
            Debug.LogError("❌ BossController chưa gán LeftHand hoặc RightHand!");
            return;
        }
        boss.StartCoroutine(RunPattern(boss));
    }

    IEnumerator RunPattern(BossController boss)
    {
        // ✊ Tay trái hoạt động
        boss.rightHand.SetIdlePose();
        yield return boss.StartCoroutine(boss.leftHand.LiftHand());
        yield return boss.StartCoroutine(boss.leftHand.SmashHand());

        yield return new WaitForSeconds(delayBetweenHands);

        // ✊ Tay phải hoạt động
        boss.leftHand.SetIdlePose();
        yield return boss.StartCoroutine(boss.rightHand.LiftHand());
        yield return boss.StartCoroutine(boss.rightHand.SmashHand());

        yield return new WaitForSeconds(delayBetweenCycles);

        // 👉 Sau khi Pattern1 kết thúc, chờ 5 giây rồi chuyển sang Pattern2
        boss.NextPattern(5f);
    }
}
