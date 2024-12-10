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
using TMPro;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System.Data;

public class DiamondCollectorOnline : MonoBehaviourPun
{
    public int score = 0;
    private GameObject attachedDiamond = null;
    public Transform followPoint;
    public float followSpeed = 5f;
    public Vector3 diamondOffset = new Vector3(0, -1f, 0);
    public DiamondSpawnerOnline diamondSpawner;
    private Text scoreText;
    public GameObject myScoreObject;
    public string name;
    public int xp = 0;
    //private GameObject myScoreObject;





    void Start (){

        Debug.LogError($"started for {PhotonNetwork.LocalPlayer.ActorNumber}");
      // myScoreObject = PhotonNetwork.Instantiate("Score", Vector3.zero, Quaternion.identity);
      PhotonNetwork.LocalPlayer.NickName = $"Player{PhotonNetwork.LocalPlayer.ActorNumber}";

   }


    void Update()
    {
        if (attachedDiamond != null)
        {
            Vector3 targetPosition = followPoint.position + diamondOffset;
            attachedDiamond.transform.position = Vector3.Lerp(attachedDiamond.transform.position, targetPosition, followSpeed * Time.deltaTime);

            PhotonView diamondPV = attachedDiamond.GetComponent<PhotonView>();
            if(diamondPV != null){
              diamondPV.RPC("SyncDiamondPosition", RpcTarget.Others, attachedDiamond.transform.position);
            }
        }
    }
    public int positionIndex(){
      return PhotonNetwork.LocalPlayer.ActorNumber - 1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Diamond") && attachedDiamond == null)
        {
            PhotonView diamondPV = collision.gameObject.GetComponent<PhotonView>();
            DiamondLogic ownership = diamondPV.GetComponent<DiamondLogic>();
            // Make sure we have authority over the diamond
            if (diamondPV != null && !ownership.IsClaimed)
            {
                Debug.LogError("Claiming onwership of diamond");
                ownership.IsClaimed = true;

                // Attach the diamond via RPC
                photonView.RPC("AttachDiamond", RpcTarget.AllBuffered, diamondPV.ViewID);

            }

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
        if (diamondPV != null && diamondPV.Owner == PhotonNetwork.LocalPlayer)
        {
          Debug.Log("diamond is owned by player, so will attach");
            attachedDiamond = diamondPV.gameObject;
            attachedDiamond.GetComponent<Collider2D>().enabled = false;
            attachedDiamond.GetComponent<Rigidbody2D>().isKinematic = true;
            attachedDiamond.transform.SetParent(this.transform);

            // UPdate diamond position across the network if necessary
            photonView.RPC("SyncDiamondPosition", RpcTarget.All, attachedDiamond.transform.position);
        }
    }

    [PunRPC]
    void SyncDiamondPosition(Vector3 newPosition){
      if(attachedDiamond != null){
          attachedDiamond.transform.position = newPosition;
      }
    }
    [PunRPC]
    void updatescore(int newscore){
      score = newscore;
    }
    void DropOffDiamond()
    {
        score++;
        photonView.RPC("updatescore", RpcTarget.AllBuffered, score);
        PhotonView myScoreObjectPV = myScoreObject.GetComponent<PhotonView>();
        if(myScoreObjectPV == null){
          Debug.LogError("ScoreObjectPV is null");
        } else {
          Debug.LogError($"ScoreObjectPV view ID -> {myScoreObjectPV.ViewID}");
        }
        myScoreObjectPV.RPC("UpdateScoreTextRPC", RpcTarget.AllBuffered, score);
        if (attachedDiamond != null)
        {
            PhotonView diamondPV = attachedDiamond.GetComponent<PhotonView>();
            DiamondLogic ownership = diamondPV.GetComponent<DiamondLogic>();
            if(diamondPV != null && ownership.IsClaimed == true){
              Debug.LogError("Setting the ownership to unclaimed now that dropeed off");
              ownership.IsClaimed = false;
            }
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

}
