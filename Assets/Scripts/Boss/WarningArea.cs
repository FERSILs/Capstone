using UnityEngine;

public class WarningArea : MonoBehaviour
{
    public float duration = 1f;
    public SpriteRenderer sr;
    private float timer;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        timer = duration;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        float alpha = Mathf.Clamp01(timer / duration);
        sr.color = new Color(1f, 0f, 0f, alpha * 0.5f);
        if (timer <= 0f)
            Destroy(gameObject);
    }
}
