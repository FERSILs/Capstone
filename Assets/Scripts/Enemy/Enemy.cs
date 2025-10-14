using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PolygonCollider2D))]
public class Enemy : MonoBehaviour
{
    public float enemy_moveSpeed = 3f;      
    public Transform target;          

    private Rigidbody2D rigid;
    private bool isChasing = false;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        rigid.gravityScale = 0f;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;

        PolygonCollider2D col = GetComponent<PolygonCollider2D>();
        col.isTrigger = true;

       
    }

    void FixedUpdate()
    {
        if (isChasing && target != null)
        {
            Vector2 dir = (target.position - transform.position).normalized;

            rigid.linearVelocity = dir * enemy_moveSpeed;
        }
        else
        {
            rigid.linearVelocity = Vector2.zero;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            target = other.transform;
            isChasing = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = false;
            target = null;
        }
    }
}
