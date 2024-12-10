using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class gamertimeronline : MonoBehaviourPun
{
  //private GameObject endGamePanel;
  Transform endGPanel;
  GameObject endGamePanel;
  private float countdownTime = 10f;
  private bool gamestart = false;
    // Start is called before the first frame update
    void Start()
    {
      // if(PhotonNetwork.IsMasterClient){
      // GameObject canvasObject = GameObject.Find("Canvas");
      //   if (canvasObject == null)
      //       {
      //         Debug.LogError("Canvas not found in the scene. Ensure a Canvas exists and is named 'Canvas'.");
      //         return;
      //   } else {
      //            Debug.LogError("Canvas found");
      //          }
      // endGamePanel = PhotonNetwork.Instantiate("endGamePanel", Vector3.zero, Quaternion.identity);
      // endGamePanel.transform.SetParent(canvasObject.transform, false);
      // PhotonView PanelPV = endGamePanel.GetComponent<PhotonView>();
      // PanelPV.RPC("setFalse", RpcTarget.AllBuffered);
      photonView.RPC("setUpTimer", RpcTarget.AllBuffered, countdownTime);
      GameObject canvasObject = GameObject.Find("Canvas");
      endGPanel = canvasObject.transform.Find("endGamePanel(Clone)");
      endGamePanel= endGPanel.gameObject;
  }

    // Update is called once per frame
    void Update()
    {
      if(PhotonNetwork.CurrentRoom.PlayerCount == 1 && !gamestart){
        StartCoroutine(StartCountdown());
        gamestart = true;
      }
    }
    [PunRPC]
    void setUpTimer(float timeremaing){
      GameObject canvasObject = GameObject.Find("Canvas");
         if (canvasObject == null)
         {
             Debug.LogError("Canvas not found in the scene. Ensure a Canvas exists and is named 'Canvas'.");
             return;
         } else {
           Debug.LogError("Canvas found");
         }
        gameObject.transform.SetParent(canvasObject.transform, false);
        Text timerText = gameObject.GetComponent<Text>();
        timerText.text = $"Time:{timeremaing}";
    }
    [PunRPC]
    void updateTimer(float newtime){
      Text timerText = gameObject.GetComponent<Text>();
      timerText.text = $"Time:{newtime}";
    }
    IEnumerator StartCountdown(){
      while(countdownTime > 0){
        yield return new WaitForSeconds(1f);
        countdownTime--;
        photonView.RPC("updateTimer", RpcTarget.AllBuffered, countdownTime);
      }
      photonView.RPC("endGame", RpcTarget.AllBuffered);
    }
    [PunRPC]
    void endGame(){
      GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
      DiamondCollectorOnline windcOnline = null;
      List<string> winningplayers = new List<string>();
      int highestscore = 0;
      string winner = "no winner";
      bool draw = false;
      foreach(GameObject player in players){
        Movement playerMovement = player.GetComponent<Movement>();
        DiamondCollectorOnline dcOnline = player.GetComponent<DiamondCollectorOnline>();
        playerMovement.enabled = false;
          if(dcOnline.score > highestscore){
            highestscore = dcOnline.score;
          }
      }
      foreach(GameObject player in players){
        DiamondCollectorOnline dcOnline = player.GetComponent<DiamondCollectorOnline>();
        if(dcOnline.score == highestscore){
          winningplayers.Add(dcOnline.name);
          if(dcOnline.score > 0){
            playerDataUpdate pdUpdate = player.GetComponent<playerDataUpdate>();
            pdUpdate.isWinner = true;
          }

        }
      }
      if(winningplayers.Count == 1){
        winner = $"winner: {winningplayers[0]}";
      }
      else if (winningplayers.Count == 2){
        winner = $"draw";
      }

      PhotonView endGamePanelPV = endGamePanel.GetComponent<PhotonView>();
      endGamePanelPV.RPC("setTrue", RpcTarget.AllBuffered);
      endGamePanelPV.RPC("winner", RpcTarget.AllBuffered, winner);
      foreach(GameObject Player in players){
        playerDataUpdate pdUpdate = Player.GetComponent<playerDataUpdate>();
        pdUpdate.gameEnd = true;
      }
    }

}
