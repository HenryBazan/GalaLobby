using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MPManager : MonoBehaviourPunCallbacks
{
    public string GameVersion = "1.0";
    private static bool isRoomCreated = false;
    private string roomName = "Room";

    [SerializeField] private TextMeshProUGUI playerListText; // UI to display player names
    [SerializeField] private TextMeshProUGUI statusText;     // UI to show status messages
    [SerializeField] private TextMeshProUGUI countdownText; // UI for countdown

    private int countdownTime = 10;
    private bool isCountingDown = false;


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
        CheckRoomStatus();
    }

    private void CheckRoomStatus()
    {
        if (isRoomCreated)
        {
            Debug.Log("A room exists. Attempting to join...");
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            Debug.Log("No room exists. Creating a room...");
            CreateRoom();
        }
    }

    private void CreateRoom()
    {
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true,
            IsOpen = true,
        };

        PhotonNetwork.CreateRoom(roomName, options);
        isRoomCreated = true;
    }

    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning("No random room found. Creating a new room...");
        CreateRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name}");
        UpdatePlayerListUI();
        statusText.text = "Waiting for other players...";

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            StartCountdown();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} has joined the room.");
        UpdatePlayerListUI();

        // Update status text if room is full
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            StartCountdown();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} has left the room.");
        UpdatePlayerListUI();

        if (PhotonNetwork.CurrentRoom.PlayerCount == 0 && PhotonNetwork.IsMasterClient)
        {
            Debug.Log("All players have left the room. Closing room...");
            PhotonNetwork.LeaveRoom();
            isRoomCreated = false;
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

    private void StartCountdown()
    {
        if (PhotonNetwork.IsMasterClient && !isCountingDown)
        {
            Debug.Log("Starting countdown...");
            photonView.RPC("StartCountdownRPC", RpcTarget.All, countdownTime);
        }
    }

    [PunRPC]
    private void StartCountdownRPC(int startTime)
    {
        StartCoroutine(CountdownCoroutine(startTime));
    }

    private System.Collections.IEnumerator CountdownCoroutine(int startTime) 
    {
        isCountingDown = true;
        int currentTime = startTime;

        while (currentTime > 0)
        {
            countdownText.text = $"Game starts in: {currentTime}";
            yield return new WaitForSeconds(1);
            currentTime--;
        }

        countdownText.text = "Starting game...";

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("OnlineMatch");
        }
    }
}
