using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform shootingPoint;
    private PlayerMovement playerMovement;

    public Vector2 upOffset = new Vector2(1, 1);
    public Vector2 downOffset = new Vector2(0, 5);
    public Vector2 leftOffset = new Vector2(5, 0);
    public Vector2 rightOffset = new Vector2(5, 0);

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        shootingPoint.localPosition = new Vector2(0, 0); 

        if (Input.GetKey(KeyCode.W))
        {
            shootingPoint.localPosition = new Vector2(0, 0);
        }
        else if (Input.GetKey(KeyCode.S)) 
        {
            shootingPoint.localPosition = new Vector2(0, 0);
        }
        else if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            shootingPoint.localPosition = new Vector2(0, 0);
        }
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            shootingPoint.localPosition = new Vector2(0, 0);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, Quaternion.identity);
        Bullet bScript = bullet.GetComponent<Bullet>();
        bScript.shooter = gameObject;
        bScript.bulletTag = "PlayerBullet";

        Vector2 direction = playerMovement.GetMovementDirection();
        bullet.transform.up = direction;
    }
}
