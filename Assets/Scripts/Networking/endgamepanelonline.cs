using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class endgamepanelonline : MonoBehaviourPun
{
  [PunRPC]
  void setFalse(){
    gameObject.SetActive(false);
  }

  [PunRPC]
  void setTrue(){
    gameObject.SetActive(true);
  }

  [PunRPC]
  void winner(string gameWinningtext){
    Transform winnerT = gameObject.transform.Find("winner");
    Text winnerText = winnerT.GetComponent<Text>();
    winnerText.text = gameWinningtext;
  }
}
