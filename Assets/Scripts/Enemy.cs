using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float detectionRange = 5f;
    public float shootCooldown = 1f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    private Transform player1;
    private Transform player2;
    private Transform targetPlayer; 
    private float nextShootTime = 0f;
    private bool canShoot = true;

    void Start()
    {
        player1 = GameObject.FindGameObjectWithTag("Player1").transform;
        player2 = GameObject.FindGameObjectWithTag("Player2").transform;
    }

    void Update()
    {
        if (canShoot)
        {
            float distanceToPlayer1 = Vector2.Distance(transform.position, player1.position);
            float distanceToPlayer2 = Vector2.Distance(transform.position, player2.position);

            if (distanceToPlayer1 <= detectionRange || distanceToPlayer2 <= detectionRange)
            {
                if (distanceToPlayer1 < distanceToPlayer2)
                {
                    targetPlayer = player1;
                }
                else
                {
                    targetPlayer = player2;
                }

                FacePlayer();
                TryShoot();
            }
        }
    }

    void TryShoot()
    {
        if (Time.time >= nextShootTime)
        {
            ShootAtPlayer();
            nextShootTime = Time.time + shootCooldown;
        }
    }

    void FacePlayer()
    {
        if (targetPlayer != null)
        {
            Vector2 directionToPlayer = (targetPlayer.position - transform.position).normalized;
            transform.up = directionToPlayer;
        }
    }

    void ShootAtPlayer()
    {
        if (targetPlayer != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Bullet bScript = bullet.GetComponent<Bullet>();
            bScript.shooter = gameObject;
            bScript.bulletTag = "EnemyBullet";

            Vector2 direction = (targetPlayer.position - firePoint.position).normalized;

            bullet.GetComponent<Rigidbody2D>().velocity = direction * bullet.GetComponent<Bullet>().speed;
            bullet.transform.up = direction;
        }
    }

    public void DisableShooting()
    {
        canShoot = false;
    }

    public void EnableShooting()
    {
        canShoot = true;
    }
}
