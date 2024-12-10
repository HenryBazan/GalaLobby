using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class AssignNickname : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
      int numPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
          if(photonView.IsMine){
              string nickname;
              switch(numPlayers)
              {
                case 1:
                  nickname = "Player1";
                  break;
                case 2:
                  nickname = "Player2";
                  break;
                case 3:
                  nickname = "Player3";
                  break;
                case 4:
                  nickname = "Player4";
                  break;
                default:
                  nickname = "UnknownPlayer";
                  break;
              }

              photonView.RPC("setNickname", RpcTarget.AllBuffered, nickname);
              Debug.LogError($"done setting player{PhotonNetwork.CurrentRoom.PlayerCount} nickname to {PhotonNetwork.LocalPlayer.NickName}");
          }
    }
    [PunRPC]
    public void setNickname(string nickname){
      PhotonNetwork.LocalPlayer.NickName = nickname;
      Debug.LogError($"Set local player nickname to {nickname}");
    }


}
