using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections.Generic;

public class PlayerStatistics : MonoBehaviour
{
    public void UpdatePlayerStatistics(int newWins, int newDiamonds)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = "Wins", Value = newWins},
                new StatisticUpdate { StatisticName = "Diamonds", Value = newDiamonds}
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnStatsUpdateSuccess, OnStatsUpdateError );
    }

    private void OnStatsUpdateSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Player statistics updated successfully");
    }

    private void OnStatsUpdateError(PlayFabError error)
    {
        Debug.LogError("Error updating player statistics:" + error.GenerateErrorReport());
    }

    public void GetPlayerStatistics()
    {
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(), OnGetStatsSuccess, OnGetStatsError);
    }

    private void OnGetStatsSuccess(GetPlayerStatisticsResult result)
    {
        foreach (var stat in result.Statistics)
        {
            Debug.Log("Statistic: " + stat.StatisticName + ", Value: " + stat.Value);
        }
    }

    private void OnGetStatsError(PlayFabError error)
    {
        Debug.LogError("Error retrieving player statistics: " + error.GenerateErrorReport());
    }
}
