using UnityEngine;
using System.Collections;

public class BossHand : MonoBehaviour
{
    public enum HandType { Left, Right }
    public HandType handType;

    [Header("Attack Settings")]
    public GameObject warningPrefab;
    public float warningTime = 0.8f;
    public float attackTime = 0.3f;
    public float liftAngle = 45f;
    public float smashAngle = -60f;

    private Transform armPivot;
    private Quaternion defaultRotation;

    void Start()
    {
        armPivot = transform.parent;
        defaultRotation = armPivot.localRotation;
    }

    public IEnumerator LiftHand()
    {
        Debug.Log($"[{handType}] Giơ tay lên");
        float t = 0;
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

    public IEnumerator SmashHand()
    {
        Debug.Log($"[{handType}] Đập tay xuống");

        if (warningPrefab != null)
        {
            Instantiate(warningPrefab, transform.position, Quaternion.identity);
        }

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

        yield return new WaitForSeconds(0.2f);
        armPivot.localRotation = defaultRotation;
    }
}
