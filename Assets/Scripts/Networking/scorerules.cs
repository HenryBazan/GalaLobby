using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
public class scorerules : MonoBehaviourPun
{
  public DiamondCollectorOnline diamondcollectoronline;

  private static Vector3[] scorePositions = {
    new Vector3(-252, 158, 0),
    new Vector3(220, 158, 0),
    new Vector3(-252,-188,0),
    new Vector3(220,-188,0)
  };
  private int scoreIndex;
  private bool setUp = false;
    // Start is called before the first frame update
    void Start()
    {
    //   if(photonView.IsMine){
    //     photonView.RPC("SetupScoreUI", RpcTarget.AllBuffered);
    // }
    }
    void Update(){
      if(diamondcollectoronline != null && photonView.IsMine && !setUp){
        photonView.RPC("SetupScoreUI", RpcTarget.AllBuffered);
        setUp = true;
        return;
      }
    }

    [PunRPC]
    void SetupScoreUI(){

      GameObject canvasObject = GameObject.Find("Canvas");
         if (canvasObject == null)
         {
             Debug.LogError("Canvas not found in the scene. Ensure a Canvas exists and is named 'Canvas'.");
             return;
         } else {
           Debug.LogError("Canvas found");
         }

      if(diamondcollectoronline == null){
        Debug.LogError("diamondcollectoronline is null ?");
      }
      if(diamondcollectoronline.positionIndex() == null){
        Debug.LogError("positionindex is null ?");
      }
      scoreIndex = diamondcollectoronline.positionIndex();
      transform.SetParent(canvasObject.transform, false);
      transform.localPosition = scorePositions[scoreIndex];
      Text scoreText = GetComponent<Text>();
      scoreText.text = $"Score: 0";
    }

    [PunRPC]
    void UpdateScoreTextRPC(int newScore){
      Text scoreText = GetComponent<Text>();
      scoreText.text = $"Score: {newScore}";
    }
}
