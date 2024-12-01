using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Lock the rotation to keep the object upright
        transform.rotation = Quaternion.Euler(0, 0, 0);

        // Initialize movement as (0, 0)
        movement = Vector2.zero;

        // Check for specific keys
        if (Input.GetKey(KeyCode.W))
        {
            movement.y = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement.y = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement.x = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement.x = 1;
        }

        // Normalize and apply speed
        movement = movement.normalized * speed;
    }
    private void FixedUpdate()
    {
        rb.velocity = movement;
    }

    public Vector2 GetMovementDirection()
    {
        return movement.normalized;
    }
}