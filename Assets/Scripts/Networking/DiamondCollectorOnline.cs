// using System.Collections;
// using UnityEngine;
// using UnityEngine.UI;
// using Photon.Pun;
// using Photon;
// public class DiamondCollector : MonoBehaviour
// {
//     //public Text scoreText;
//     private int score = 0;
//     private GameObject attachedDiamond = null;
//     public Transform followPoint;
//     public float followSpeed = 5f;
//     public Vector3 diamondOffset = new Vector3(0, -1f, 0);
//     public DiamondSpawner diamondSpawner;
//
//     void Start()
//     {
//         //UpdateScoreText();
//     }
//
//     void Update()
//     {
//         if (attachedDiamond != null)
//         {
//             Vector3 targetPosition = followPoint.position + diamondOffset;
//             attachedDiamond.transform.position = Vector3.Lerp(attachedDiamond.transform.position, targetPosition, followSpeed * Time.deltaTime);
//         }
//     }
//
//     private void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (collision.gameObject.CompareTag("Diamond") && attachedDiamond == null)
//         {
//             AttachDiamond(collision.gameObject);
//         }
//         else if (collision.gameObject.CompareTag("Area") && attachedDiamond != null)
//         {
//             DropOffDiamond();
//         }
//     }
//
//     void AttachDiamond(GameObject diamond)
//     {
//         attachedDiamond = diamond;
//         attachedDiamond.GetComponent<Collider2D>().enabled = false;
//         attachedDiamond.GetComponent<Rigidbody2D>().isKinematic = true;
//         attachedDiamond.transform.SetParent(this.transform);
//     }
//
//     void DropOffDiamond()
//     {
//         score++;
//         UpdateScoreText();
//
//         attachedDiamond.GetComponent<SpriteRenderer>().enabled = false;
//         attachedDiamond.GetComponent<Collider2D>().enabled = false;
//
//         StartCoroutine(Respawn(attachedDiamond, 3f));
//
//         attachedDiamond = null;
//     }
//
//     IEnumerator Respawn(GameObject diamond, float delay)
//     {
//         yield return new WaitForSeconds(delay);
//
//         diamond.GetComponent<SpriteRenderer>().enabled = true;
//         diamondSpawner.RespawnDiamond(diamond);
//     }
//
//     public int GetScore()
//     {
//         return score;
//     }
//
//     void UpdateScoreText()
//     {
//         //scoreText.text = "Score: " + score;
//     }
// }




using System.Collections;
using UnityEngine;
using Photon.Pun;

public class DiamondCollectorOnline : MonoBehaviourPun
{
    private int score = 0;
    private GameObject attachedDiamond = null;
    public Transform followPoint;
    public float followSpeed = 5f;
    public Vector3 diamondOffset = new Vector3(0, -1f, 0);
    public DiamondSpawnerOnline diamondSpawner;

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
            PhotonView diamondPV = collision.gameObject.GetComponent<PhotonView>();

            // Make sure we have authority over the diamond
            if (diamondPV != null && !diamondPV.IsMine)
            {
                diamondPV.RequestOwnership();
            }

            // Attach the diamond via RPC
            photonView.RPC("AttachDiamond", RpcTarget.AllBuffered, diamondPV.ViewID);
        }
        else if (collision.gameObject.CompareTag("Area") && attachedDiamond != null)
        {
            DropOffDiamond();
        }
    }

    [PunRPC]
    void AttachDiamond(int diamondViewID)
    {
        PhotonView diamondPV = PhotonView.Find(diamondViewID);
        if (diamondPV != null)
        {
            attachedDiamond = diamondPV.gameObject;
            attachedDiamond.GetComponent<Collider2D>().enabled = false;
            attachedDiamond.GetComponent<Rigidbody2D>().isKinematic = true;
            attachedDiamond.transform.SetParent(this.transform);
        }
    }

    void DropOffDiamond()
    {
        score++;
        UpdateScoreText();

        if (attachedDiamond != null)
        {
            attachedDiamond.GetComponent<SpriteRenderer>().enabled = false;
            attachedDiamond.GetComponent<Collider2D>().enabled = false;

            StartCoroutine(RespawnDiamond(attachedDiamond, 3f));

            attachedDiamond = null;
        }
    }

    IEnumerator RespawnDiamond(GameObject diamond, float delay)
    {
        yield return new WaitForSeconds(delay);


        diamond.GetComponent<SpriteRenderer>().enabled = true;
        diamond.GetComponent<Collider2D>().enabled = true;


        if (PhotonNetwork.IsMasterClient)
        {
            diamondSpawner.RespawnDiamond(diamond);
        }
    }

    public int GetScore()
    {
        return score;
    }

    void UpdateScoreText()
    {
        // Implement your UI update logic here (e.g., sending score to a UI element)
    }
}
