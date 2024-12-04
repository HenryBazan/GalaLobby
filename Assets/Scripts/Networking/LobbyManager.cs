using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI playerListText; // UI element for displaying player list

    void Start()
    {
        UpdatePlayerList(); // Initial list update
    }

    // Called when a player joins the room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} joined the lobby.");
        UpdatePlayerList();
    }

    // Called when a player leaves the room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} left the lobby.");
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        if (!PhotonNetwork.InRoom)
        {
            Debug.LogWarning("Not currently in a Photon room.");
            return;
        }

        // Clear the player list text
        playerListText.text = "Players in Lobby:\n";

        // Add all players in the room
        foreach (var player in PhotonNetwork.PlayerList)
        {
            playerListText.text += $"{player.NickName}\n";
        }
    }
}
