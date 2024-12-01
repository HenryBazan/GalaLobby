using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform shootingPoint;
    private PlayerMovement playerMovement;

    // Offsets for shooting point
    public Vector2 upOffset = new Vector2(100, 100);
    public Vector2 downOffset = new Vector2(0, -10);
    public Vector2 leftOffset = new Vector2(-10, 0);
    public Vector2 rightOffset = new Vector2(10, 0);

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // Initialize shooting point
        shootingPoint.localPosition = new Vector2(0, 2); // Default position

        // Check movement direction
        if (Input.GetKey(KeyCode.W)) // Up
        {
            shootingPoint.localPosition = new Vector2(0, 2);
        }
        else if (Input.GetKey(KeyCode.S)) // Down
        {
            shootingPoint.localPosition = new Vector2(0, -2);
        }
        else if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) // Left only
        {
            shootingPoint.localPosition = new Vector2(-2, 0);
        }
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) // Right only
        {
            shootingPoint.localPosition = new Vector2(2, 0);
        }

        // Check for shooting input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, Quaternion.identity);
        Vector2 direction = playerMovement.GetMovementDirection();
        bullet.transform.up = direction;
    }
}
