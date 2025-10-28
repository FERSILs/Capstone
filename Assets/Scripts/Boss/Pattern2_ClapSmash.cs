using UnityEngine;
using System.Collections;

public class Pattern2_ClapSmash : BossPatternBase
{
    [Header("Pattern Settings")]
    public float spreadAngle = 60f;     // Góc dang tay ra ngoài
    public float spreadTime = 0.5f;     // Thời gian dang tay
    public float smashTime = 0.3f;      // Thời gian đập vào
    public float returnTime = 0.5f;     // Thời gian quay lại idle
    public float delayBeforeSmash = 0.4f; // Dừng chút trước khi đập
    public float delayAfter = 1.2f;     // Nghỉ sau khi kết thúc
    public GameObject impactEffect;     // Hiệu ứng khi 2 tay đập vào giữa

    public override void StartPattern(BossController boss)
    {
        boss.StartCoroutine(RunPattern(boss));
    }

    IEnumerator RunPattern(BossController boss)
    {
        var leftArm = boss.leftHand.GetArmPivot();
        var rightArm = boss.rightHand.GetArmPivot();

        Quaternion leftDefault = boss.leftHand.GetDefaultRotation();
        Quaternion rightDefault = boss.rightHand.GetDefaultRotation();

        Quaternion leftSpread = Quaternion.Euler(0, 0, spreadAngle);
        Quaternion rightSpread = Quaternion.Euler(0, 0, -spreadAngle);

        // 1️⃣ Dang hai tay ra ngoài
        Debug.Log("Pattern2: Dang tay ra ngoài!");
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / spreadTime;
            float eased = Mathf.SmoothStep(0, 1, t);
            leftArm.localRotation = Quaternion.Lerp(leftDefault, leftSpread, eased);
            rightArm.localRotation = Quaternion.Lerp(rightDefault, rightSpread, eased);
            yield return null;
        }

        // 2️⃣ Giữ tư thế dang tay một chút
        yield return new WaitForSeconds(delayBeforeSmash);

        // 3️⃣ Cùng đập vào giữa
        Debug.Log("Pattern2: Đập tay vào giữa!");
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / smashTime;
            float eased = Mathf.SmoothStep(0, 1, t);
            leftArm.localRotation = Quaternion.Lerp(leftSpread, leftDefault, eased);
            rightArm.localRotation = Quaternion.Lerp(rightSpread, rightDefault, eased);
            yield return null;
        }

        // 💥 4️⃣ Va chạm ở giữa (hiệu ứng tùy chọn)
        if (impactEffect != null)
        {
            Instantiate(impactEffect, boss.transform.position, Quaternion.identity);
        }
        else if (boss.leftHand.warningPrefab != null)
        {
            Instantiate(boss.leftHand.warningPrefab, boss.transform.position, Quaternion.identity);
        }

        // 5️⃣ Tay quay về tư thế mặc định mượt mà
        yield return new WaitForSeconds(delayAfter);

        // 6️⃣ Sang pattern kế (hoặc lặp lại)
        boss.NextPattern(5f); // chờ 5s rồi quay lại pattern 1
    }
}
