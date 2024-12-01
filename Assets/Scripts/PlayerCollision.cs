using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Walls")
        {
            rb.velocity = Vector2.zero;
            Debug.Log("Collided with Wall!");
        }
    }
}