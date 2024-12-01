using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        // Connect to Photon if not already connected
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "1.0"; // Replace with your game version
        }
    }

    public void PlayGame()
    {
        // Load the single-player game scene
        SceneManager.LoadSceneAsync(2); //Load local game
    }

    public void PlayOnlineGame()
    {
        // Check if connected to Photon before transitioning
        if (PhotonNetwork.IsConnected)
        {
            SceneManager.LoadSceneAsync(3); // Load multiplayer lobby scene index
        }
        else
        {
            Debug.LogWarning("Connecting to Photon... Please wait.");
            StartCoroutine(WaitForPhotonConnection());
        }
    }

    private IEnumerator WaitForPhotonConnection()
    {
        while (!PhotonNetwork.IsConnected)
        {
            yield return null; // Wait until connected
        }
        SceneManager.LoadSceneAsync(3); // Load the multiplayer lobby scene
    }
}
