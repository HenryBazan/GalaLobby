using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int health = 15; 
    public float respawnTime = 15f; 
    private Vector3 initialPosition; 
    private bool isDead = false;  

    private Renderer enemyRenderer;
    private Collider2D enemyCollider;

    void Start()
    {
        initialPosition = transform.position; 
        enemyRenderer = GetComponent<Renderer>();  
        enemyCollider = GetComponent<Collider2D>(); 
    }

    public void TakeDamage()
    {
        health--;
        Debug.Log(health);
        if (health <= 0 && !isDead)
        {
            KillEnemy();
        }
    }

    private void KillEnemy()
    {
        isDead = true;
        enemyRenderer.enabled = false;  
        enemyCollider.enabled = false;

        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.DisableShooting();
        }

        Invoke("RespawnEnemy", respawnTime);
    }

    private void RespawnEnemy()
    {
        transform.position = initialPosition;
        health = 15; 
        isDead = false;

        enemyRenderer.enabled = true;
        enemyCollider.enabled = true;

        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.EnableShooting();
        }

    }
}
