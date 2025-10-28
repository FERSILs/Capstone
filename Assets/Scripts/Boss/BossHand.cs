using UnityEngine;
using System.Collections;

public class BossHand : MonoBehaviour
{
    public enum HandType { Left, Right }
    public HandType handType;

    [Header("Attack Settings")]
    public GameObject warningPrefab;
    public float warningTime = 0.8f;      // Thời gian hiển thị cảnh báo
    public float attackTime = 0.3f;       // Thời gian tay đập xuống
    public float returnTime = 0.5f;       // Thời gian quay lại vị trí ban đầu
    public float liftAngle = 45f;         // Góc giơ tay
    public float smashAngle = -60f;       // Góc đập tay

    private Transform armPivot;
    private Quaternion defaultRotation;
    public Transform GetArmPivot() => armPivot;
    public Quaternion GetDefaultRotation() => defaultRotation;

    void Awake()
    {
        armPivot = transform.parent;
        if (armPivot == null)
            Debug.LogError($"{name}: Không tìm thấy ArmPivot (cha của tay)!");
        else
            defaultRotation = armPivot.localRotation;
    }

    //void Start()
    //{
    //    armPivot = transform.parent;
    //    if (armPivot == null)
    //    {
    //        Debug.LogError($"{name}: BossHand cần nằm trong ArmLeft/ArmRight!");
    //        return;
    //    }
    //    defaultRotation = armPivot.localRotation;
    //}

    // ✋ Giơ tay lên
    public IEnumerator LiftHand()
    {
        float t = 0f;
        float duration = 0.3f;
        Quaternion startRot = armPivot.localRotation;
        Quaternion targetRot = Quaternion.Euler(0, 0, liftAngle * (handType == HandType.Left ? 1 : -1));

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            armPivot.localRotation = Quaternion.Lerp(startRot, targetRot, t);
            yield return null;
        }
    }

    // 👊 Đập tay xuống
    public IEnumerator SmashHand()
    {
        if (warningPrefab != null)
            Instantiate(warningPrefab, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(warningTime);

        float t = 0;
        Quaternion startRot = armPivot.localRotation;
        Quaternion targetRot = Quaternion.Euler(0, 0, smashAngle * (handType == HandType.Left ? 1 : -1));

        while (t < 1f)
        {
            t += Time.deltaTime / attackTime;
            armPivot.localRotation = Quaternion.Lerp(startRot, targetRot, t);
            yield return null;
        }

        // 🌀 Sau khi đập xong → quay lại vị trí ban đầu mượt
        yield return StartCoroutine(ReturnToDefault());
    }

    // 🌀 Quay lại vị trí nghỉ mượt
    public IEnumerator ReturnToDefault()
    {
        float t = 0f;
        Quaternion startRot = armPivot.localRotation;
        while (t < 1f)
        {
            t += Time.deltaTime / returnTime;
            armPivot.localRotation = Quaternion.Lerp(startRot, defaultRotation, t);
            yield return null;
        }
    }

    // ✋ Đưa về tư thế nghỉ ngay lập tức (khi tay còn lại ra đòn)
    public void SetIdlePose()
    {
        armPivot.localRotation = defaultRotation;
    }
}
