using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MPManager : MonoBehaviourPunCallbacks
{
    public string GameVersion = "1.0";
    private string lobbyName = "GlobalLobby"; // Persistent lobby name

    [SerializeField] private TextMeshProUGUI playerListText; // UI to display player names
    [SerializeField] private TextMeshProUGUI statusText;     // UI to show status messages
    [SerializeField] private TextMeshProUGUI countdownText; // UI for countdown

    private int countdownTime = 10;
    private bool isCountingDown = false;

    void Start()
    {
        // Connect to Photon when the player logs in
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = GameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master server.");
        CreateOrAccessPersistentLobby(); // Ensure a persistent lobby exists
    }

    private void CreateOrAccessPersistentLobby()
    {
        Debug.Log("Creating or ensuring the existence of the global lobby...");

        // Check if a lobby already exists. If not, create one
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2, // Limit the lobby to 2 players
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.CreateRoom(lobbyName, options); // Will fail if the room already exists
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Global lobby already exists. Joining the existing lobby...");
        PhotonNetwork.JoinRoom(lobbyName); // Join the existing lobby
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name}");
        UpdatePlayerListUI();

        // If the lobby is full, start the game
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            StartCountdown();
        }
        else
        {
            statusText.text = "Waiting for other players...";
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} has joined the room.");
        UpdatePlayerListUI();

        // Start the game when the lobby is full
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            StartCountdown();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} has left the room.");
        UpdatePlayerListUI();
        statusText.text = "Waiting for other players...";
        isCountingDown = false; // Stop countdown if a player leaves
    }

    private void UpdatePlayerListUI()
    {
        playerListText.text = "Players in Lobby:\n";

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
            // Transition both players to the game scene
            PhotonNetwork.LoadLevel("OnlineMatch"); // Replace with your actual game scene name
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"Disconnected from Photon: {cause}");
        isCountingDown = false;
        PhotonNetwork.ConnectUsingSettings(); // Reconnect to Photon if disconnected
    }
}
