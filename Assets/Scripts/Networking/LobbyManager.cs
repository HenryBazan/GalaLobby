using UnityEngine;
using TMPro;
using Photon.Pun;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerListText; // UI element for displaying player list

    void Start()
    {
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        // Clear the current player list
        playerListText.text = "Players in Lobby:\n";

        // Add the local player's username from PlayerPrefs
        string localUsername = PlayerPrefs.GetString("PlayerUsername", "Unknown Player");
        playerListText.text += $"{localUsername}\n";

        // If using Photon, add other players in the room
        if (PhotonNetwork.InRoom)
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.NickName != localUsername) // Avoid duplicate entry
                {
                    playerListText.text += $"{player.NickName}\n";
                }
            }
        }
    }
}
