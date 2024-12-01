using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DiamondCollector : MonoBehaviour
{
    public Text scoreText;
    private int score = 0;
    private GameObject attachedDiamond = null;
    public Transform followPoint;
    public float followSpeed = 5f;
    public Vector3 diamondOffset = new Vector3(0, -1f, 0);
    public DiamondSpawner diamondSpawner;

    public static int player1Score = 0;
    public static int player2Score = 0;

    void Start()
    {
        UpdateScoreText();
    }

    void Update()
    {
        if (attachedDiamond != null)
        {
            Vector3 targetPosition = followPoint.position + diamondOffset;
            attachedDiamond.transform.position = Vector3.Lerp(attachedDiamond.transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Diamond") && attachedDiamond == null)
        {
            AttachDiamond(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Area") && attachedDiamond != null)
        {
            DropOffDiamond();
        }
    }

    void AttachDiamond(GameObject diamond)
    {
        attachedDiamond = diamond;
        attachedDiamond.GetComponent<Collider2D>().enabled = false;
        attachedDiamond.GetComponent<Rigidbody2D>().isKinematic = true;
        attachedDiamond.transform.SetParent(this.transform);
    }

    void DropOffDiamond()
    {
        if (gameObject.CompareTag("Player1"))
        {
            score++;
            player1Score = score;
        }
        else if (gameObject.CompareTag("Player2"))
        {
            score++;
            player2Score = score;
        }

        UpdateScoreText();

        attachedDiamond.GetComponent<SpriteRenderer>().enabled = false;
        attachedDiamond.GetComponent<Collider2D>().enabled = false;

        StartCoroutine(Respawn(attachedDiamond, 3f));

        attachedDiamond = null;
    }

    IEnumerator Respawn(GameObject diamond, float delay)
    {
        yield return new WaitForSeconds(delay);
        diamond.GetComponent<SpriteRenderer>().enabled = true;
        diamondSpawner.RespawnDiamond(diamond);
    }

    public int GetScore()
    {
        return score;
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }
}
