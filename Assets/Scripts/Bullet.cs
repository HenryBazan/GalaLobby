using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            // Example: Call a method on the player script to handle damage
            PlayerHealth playerHealth = collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1); // Adjust damage as needed
            }
            Destroy(gameObject); // Destroy bullet after hitting a player
            return; // Ignore collisions with players
        }

        Destroy(gameObject); // Destroy bullet on hit with other objects
    }

}
