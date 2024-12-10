using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PlayFabSignUp : MonoBehaviour
{
    [SerializeField] GameObject signUpTab, logInTab;
    [SerializeField] TMP_InputField username, userEmail, UserPassword, userEmailLogin, UserPasswordLogin;
    string encrytedPassword;

    public void Start()
    {
        // Set the focus on the first input field based on the active tab
        if (signUpTab.activeSelf)
        {
            username.Select();
        }
        else if (logInTab.activeSelf)
        {
            userEmailLogin.Select();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Tab key navigation logic
            if (signUpTab.activeSelf)
            {
                if (username.isFocused)
                {
                    userEmail.Select();
                }
                else if (userEmail.isFocused)
                {
                    UserPassword.Select();
                }
                else if (UserPassword.isFocused)
                {
                    username.Select(); // Loop back to the first field
                }
            }
            else if (logInTab.activeSelf)
            {
                if (userEmailLogin.isFocused)
                {
                    UserPasswordLogin.Select();
                }
                else if (UserPasswordLogin.isFocused)
                {
                    userEmailLogin.Select(); // Loop back to the first field
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // Return key activation logic
            if (signUpTab.activeSelf)
            {
                Register(); // Activate signup when on the signup tab
            }
            else if (logInTab.activeSelf)
            {
                Login(); // Activate login when on the login tab
            }
        }
    }

    public void Register()
    {
        var registerRequest = new RegisterPlayFabUserRequest { Username = username.text, Email = userEmail.text, Password = Encrypt(UserPassword.text) };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterRequestSuccess, OnRegisterFailure);
    }

    void OnRegisterRequestSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("User Registered");
    }

    void OnRegisterFailure(PlayFabError error)
    {
        Debug.Log($"User Registration Failed: {error.ErrorMessage}");
    }

    public void Login()
    {
        var request = new LoginWithEmailAddressRequest { Email = userEmailLogin.text, Password = Encrypt(UserPasswordLogin.text) };
        PlayFabClientAPI.LoginWithEmailAddress(request, LoginSuccess, LoginFailure);
    }

    public void LoginSuccess(LoginResult result)
    {
        Debug.Log("Signed In");

        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountInfoSuccess, OnGetAccountInfoFailure);

    }

    void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        string username = result.AccountInfo.Username;
        PlayerPrefs.SetString("PlayerUsername", username);
        PlayerPrefs.Save();

        Debug.Log($"Player Username: {username}");

        PhotonNetwork.NickName = username;

        SceneManager.LoadSceneAsync(1);
    }

    void OnGetAccountInfoFailure(PlayFabError error)
    {
        Debug.LogError($"Failed to retrieve account info: {error.ErrorMessage}");
    }

    public void LoginFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    public void SignUpTab()
    {
        signUpTab.SetActive(true);
        logInTab.SetActive(false);
        username.Select(); // Set focus to the first field in the signup tab
    }

    public void LogInTab()
    {
        logInTab.SetActive(true);
        signUpTab.SetActive(false);
        userEmailLogin.Select(); // Set focus to the first field in the login tab
    }

    string Encrypt(string pw)
    {
        System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] epw = System.Text.Encoding.UTF8.GetBytes(pw);
        epw = x.ComputeHash(epw);
        System.Text.StringBuilder s = new System.Text.StringBuilder();
        foreach (byte b in epw)
        {
            s.Append(b.ToString("x2").ToLower());
        }
        return s.ToString();
    }
}