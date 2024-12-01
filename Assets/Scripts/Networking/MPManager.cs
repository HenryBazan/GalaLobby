using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MPManager : MonoBehaviourPunCallbacks
{
    public string GameVersion = "1.0";
    private string roomName = "Room";

    [SerializeField] private TextMeshProUGUI playerListText; // UI to display player names
    [SerializeField] private TextMeshProUGUI statusText;     // UI to show status messages

    void Start()
    {
        // Connect to Photon if not already connected
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = GameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master server. Joining or creating a room...");
        JoinOrCreateRoom();
    }

    private void JoinOrCreateRoom()
    {
        // Attempt to join a random room
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning("No random room found. Creating a new room...");

        // Create a new room if no random room is found
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2 // Limit the room to 2 players
        };
        PhotonNetwork.CreateRoom(roomName + Random.Range(0, 10000), options);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name}");
        UpdatePlayerListUI();
        statusText.text = "Waiting for other players...";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} has joined the room.");
        UpdatePlayerListUI();

        // Update status text if room is full
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            statusText.text = "Game will start shortly...";
        }
    }

    private void UpdatePlayerListUI()
    {
        // Clear the current player list
        playerListText.text = "Players in Lobby:\n";

        // Add all players in the room
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += $"{player.NickName}\n";
        }
    }
}
