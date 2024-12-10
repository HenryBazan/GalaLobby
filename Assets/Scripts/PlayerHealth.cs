using PlayFab.EconomyModels;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public Text healthText;

    private Vector3 initPos;
    private bool isDead = false;
    private float respawnTime = 15f;

    private Renderer playerRenderer;
    private Collider2D playerCollider;
    private PlayerMovement playerMovement;
    private PlayerShooting playerShooting;

    void Start()
    {
        initPos = transform.position;
        currentHealth = maxHealth;
        UpdateHealthUI();

        playerRenderer = GetComponent<Renderer>();
        playerCollider = GetComponent<Collider2D>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponent<PlayerShooting>();

    }

    public void TakeDamage(int amount)
    {
        if(isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth;
        }
    }

    private void Die()
    {
       isDead = true;
       playerRenderer.enabled = false;
       playerCollider.enabled = false;
       playerMovement.enabled = false;
       playerShooting.enabled = false;

       Invoke("RespawnPlayer", respawnTime);
       
    }

    private void RespawnPlayer()
    {
        transform.position = initPos;
        currentHealth = maxHealth;
        isDead = false;

        playerRenderer.enabled = true;
        playerCollider.enabled = true;
        playerMovement.enabled = true;
        playerShooting.enabled = true;

        UpdateHealthUI();
    }
}
