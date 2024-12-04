using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MPManager : MonoBehaviourPunCallbacks
{
    public string GameVersion = "1.0";
    private string roomName = "GlobalRoom"; // Single persistent room name

    [SerializeField] private TextMeshProUGUI playerListText; // UI to display player names
    [SerializeField] private TextMeshProUGUI statusText;     // UI to show status messages

    private bool isTryingToJoinRoom = false; // Prevent multiple room creation attempts

    void Start()
    {
        // Set player nickname
        if (string.IsNullOrEmpty(PhotonNetwork.NickName))
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("PlayerUsername", "Player" + Random.Range(0, 1000));
        }

        // Connect to Photon if not already connected
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = GameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master server. Ready to join a room.");
        statusText.text = "Connected to Photon. Click Online Play to start.";
    }

    public void PlayOnlineGame()
    {
        if (isTryingToJoinRoom) return; // Prevent duplicate join attempts
        isTryingToJoinRoom = true;

        statusText.text = "Joining or creating a room...";
        Debug.Log("Attempting to join or create the global room...");

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Successfully joined room: {PhotonNetwork.CurrentRoom.Name}");
        isTryingToJoinRoom = false;

        UpdatePlayerListUI();
        statusText.text = "Waiting for other players...";

        // Check if the room is full
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            statusText.text = "Game will start shortly...";
            StartGame();
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Failed to join room: {message}. Retrying...");
        isTryingToJoinRoom = false;

        // Retry joining the room
        PlayOnlineGame();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} has joined the room.");
        UpdatePlayerListUI();

        // Start the game when room is full
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            statusText.text = "Game will start shortly...";
            StartGame();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} has left the room.");
        UpdatePlayerListUI();
        statusText.text = "Waiting for other players...";
    }

    private void UpdatePlayerListUI()
    {
        playerListText.text = "Players in Lobby:\n";

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += $"{player.NickName}\n";
        }
    }

    private void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Master Client initiates the game
            PhotonNetwork.LoadLevel("OnlineMatch"); // Replace with your game scene name
        }
    }
}
