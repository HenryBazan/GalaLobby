using UnityEngine;

public class SecondPlayerMovement : MonoBehaviour
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

        // Check for arrow keys
        if (Input.GetKey(KeyCode.UpArrow))
        {
            movement.y = 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            movement.y = -1;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movement.x = -1;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            movement.x = 1;
        }

        // Normalize and apply speed
        movement = movement.normalized * speed;
    }

    private void FixedUpdate()
    {
        // Apply movement to the Rigidbody2D
        rb.velocity = movement;
    }
}