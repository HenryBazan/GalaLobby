using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System.Data;
public class playerDataUpdate : MonoBehaviourPun
{

  public bool gameEnd = false;
  public bool isWinner = false;
  bool hasUpdated = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      if(gameEnd & !hasUpdated){
        hasUpdated = true;
        updateGamesPlayed();
        if(isWinner){
          updateXP(1);
        }
      }
    }
    public void updateGamesPlayed(){
      if(photonView.IsMine){
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
        result => {
          int numGames = 0;
          if(result.Data != null & result.Data.ContainsKey("GamesPlayed")){
              int.TryParse(result.Data["GamesPlayed"].Value, out numGames);
          }
          numGames+= 1;

          var updateGamesPlayedRequest = new UpdateUserDataRequest{
              Data = new Dictionary<string, string>
              {
                {"GamesPlayed", numGames.ToString()}
              }
          };

          PlayFabClientAPI.UpdateUserData(updateGamesPlayedRequest,
          Updaterequest => {
            Debug.LogError("GamesPlayed incremented on database successfully");
            },
          error => {
            Debug.LogError("GamesPlayed increment Error: " + error.GenerateErrorReport());
            });

          },
        Error => {
          Debug.LogError("Error retrieving User Data: " + Error.GenerateErrorReport());
          });
      }
    }

    public void updateXP(int xp){
      if(photonView.IsMine){
        if(xp > 0){
          PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
          result => {
            int oldXP = 0;
            if(result.Data != null & result.Data.ContainsKey("XP")){
                int.TryParse(result.Data["XP"].Value, out oldXP);
            }

            int newXP = oldXP + xp;

            var UpdateXPrequest = new UpdateUserDataRequest{
              Data = new Dictionary<string, string>
              {
                {"XP", newXP.ToString()}
              }
            };

            PlayFabClientAPI.UpdateUserData(UpdateXPrequest,
            Updaterequest => {
              Debug.LogError("XP incremented on database successfully");
              },
            error => {
              Debug.LogError("Xp increment Error: " + error.GenerateErrorReport());
              });
            },
          Error => {
            Debug.LogError("Error retrieving User Data: " + Error.GenerateErrorReport());
            });
        }
    }
  }
}
