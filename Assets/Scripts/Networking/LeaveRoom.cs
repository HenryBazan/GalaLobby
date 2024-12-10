using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using System.Collections;

public class LeaveRoom : MonoBehaviourPun
{
  GameObject MPManager = null;
  MPManager mpClass = null;
  void Start(){
    MPManager = GameObject.Find("MPManager");
    if(MPManager != null){
      mpClass = MPManager.GetComponent<MPManager>();
    }
  }
  public void LeaveRoomOnline (){
    mpClass.ReturnToMainMenu();
}
}
