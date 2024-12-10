using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Data;

public class GameTimer : MonoBehaviour
{
    public Text timerText;
    public GameObject endGamePanel;
    public Button mainMenuButton;
    public Text winnerText; 
    public PlayerMovement Movement;

    private float countdownTime = 5f;

    void Start()
    {
        UpdateTimerText();
        endGamePanel.SetActive(false);
        StartCoroutine(StartCountdown());
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    IEnumerator StartCountdown()
    {
        while (countdownTime > 0)
        {
            yield return new WaitForSeconds(1f);
            countdownTime--;
            UpdateTimerText();
        }

        EndGame();
    }

    void UpdateTimerText()
    {
        timerText.text = "Time: " + Mathf.Ceil(countdownTime).ToString() + "s";
    }

    public void IncPlayerNumGames(int gamewins, int diamonds)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                int currentGames = 0;
                int wins = 0;
                int diamondsCollected = 0;

                if (result.Data != null)
                {
                    if (result.Data.ContainsKey("GamesPlayed"))
                    {
                        int.TryParse(result.Data["GamesPlayed"].Value, out currentGames);
                    }

                    if (result.Data.ContainsKey("Wins"))
                    {
                        int.TryParse(result.Data["Wins"].Value, out wins);
                    }

                    if (result.Data.ContainsKey("DiamondsCollected"))
                    {
                        int.TryParse(result.Data["DiamondsCollected"].Value, out diamondsCollected);
                    }
                }
                
                currentGames =+ 1;
                wins =+ gamewins;
                diamondsCollected =+ diamonds;

                var updateRequest = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string,string>
                    {
                        {"GamesPlayed", currentGames.ToString()},
                        {"Wins", wins.ToString()},
                        {"DiamondsCollected", diamondsCollected.ToString()}
                    }
                };
                PlayFabClientAPI.UpdateUserData(updateRequest,
                    updateRequest =>
                    {
                        Debug.Log("User Date updated incremented and successfully");
                    },
                    error =>
                    {
                        Debug.LogError("Error updating User Data: " + error.GenerateErrorReport());
                    });
            },
            Error =>
            {
                Debug.LogError("Error retrieving User Data: " + Error.GenerateErrorReport());
            });

    }

    void EndGame()
    {
        Movement.enabled = false;
        endGamePanel.SetActive(true); 

        int gamewins = 0;
        int diamonds = DiamondCollector.player1Score + DiamondCollector.player2Score;

        if (DiamondCollector.player1Score > DiamondCollector.player2Score)
            winnerText.text = "Winner: Player 1!";
        else if (DiamondCollector.player2Score > DiamondCollector.player1Score)
            winnerText.text = "Winner: Player 2!";
        else
            winnerText.text = "It's a tie!";

        DiamondCollector.player1Score = 0;
        DiamondCollector.player2Score = 0;

        IncPlayerNumGames(gamewins, diamonds);
    }

    void ReturnToMainMenu()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
