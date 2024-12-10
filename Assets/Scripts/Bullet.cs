using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 2f;
    public GameObject shooter;

    public string bulletTag;

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
        if (collider.CompareTag("Area") || collider.gameObject == shooter)
        {
            return;
        }

        if (collider.CompareTag("Player1"))
        {
            PlayerHealth playerHealth = collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10);
            }
        }

        if(collider.CompareTag("Enemy") && bulletTag == "PlayerBullet")
        {
            EnemyDamage enemyH = collider.GetComponent<EnemyDamage>();
            if(enemyH != null)
            {
                enemyH.TakeDamage();
            }
        }

        Destroy(gameObject);
    }

}
