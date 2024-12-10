using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Linq;
using System;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private GameObject leaderboardEntryPrefab;
    [SerializeField] private Transform leaderboardContainer;

    [SerializeField]
    private List<TextMeshProUGUI> names;

    void Start()
    {
        GetLeaderboard();
    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "wins",
            StartPosition = 0,
            MaxResultsCount = 100
        };

        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardRetrieved, OnError);
    }

    private void OnLeaderboardRetrieved(GetLeaderboardResult result)
    {
        List<PlayerLeaderboardEntry> entries = result.Leaderboard;

        entries = entries.OrderByDescending(e => e.StatValue)
                  .ThenByDescending(e => GetPlayerTotalScore(e.PlayFabId))
                  .ToList();

        DisplayLeaderboard(entries);
    }

    private int GetPlayerTotalScore(string PlayFabId)
    {
        return 0;
    }

    private void DisplayLeaderboard(List<PlayerLeaderboardEntry> entries)
    {
        foreach (Transform child in leaderboardContainer)
        {
            Destroy(child.gameObject);
        }
        /*
        foreach (var entry in entries)
        {
            var newEntry = Instantiate(leaderboardEntryPrefab, leaderboardContainter);
            var entryUI = newEntry.GetComponent<LeaderboardEntryUI>();
            entryUI.SetEntry(entry.DisplayName, entry.StatValue, GetPlayerTotalScore(entry.PlayFabId));
        }
        */
        
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("Error retrieving leaderboard: " + error.GenerateErrorReport());
    }


}

