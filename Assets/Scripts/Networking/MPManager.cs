using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MPManager : MonoBehaviourPunCallbacks
{
    public string GameVersion = "1.0";
    private string roomName = "DefaultRoom";

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
        Debug.Log("Connected to Master server. Checking for existing rooms...");
        JoinOrCreateRoom();
    }

    private void JoinOrCreateRoom()
    {
        ExitGames.Client.Photon.Hashtable expectedProperties = new ExitGames.Client.Photon.Hashtable
        {
            { "RoomType", "Lobby" }
        };

        // Try to join a room with the specific "Lobby" property
        PhotonNetwork.JoinRandomRoom(expectedProperties, 0);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning("No matching room found. Creating a new room...");

        // Create a new room with custom properties
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true,
            IsOpen = true,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "RoomType", "Lobby" }
            },
            CustomRoomPropertiesForLobby = new string[] { "RoomType" } // Allow "RoomType" to be visible in the lobby
        };

        PhotonNetwork.CreateRoom(roomName + Random.Range(0, 10000), options);
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

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            StartCountdown();
        }
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
            PhotonNetwork.LoadLevel("OnlineMatch"); // Replace with your actual game scene name
        }
    }
}
