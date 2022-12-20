using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Auth;
using TMPro;

public class GameMenuManager : MonoBehaviour
{
    public AuthManager authMgr;
    public GameObject signOutBtn;

    public GameObject playBtn;

    //UI
    public TextMeshProUGUI displayName;

    public void Awake()
    {
        InitializeFirebase();

        displayName.text = "Player: " + authMgr.GetCurrentUserDisplayName();
    }
    public void InitializeFirebase()
    {

    }

    public void SignOut()
    {
        authMgr.SignOutUser();
    }

    public void SwitchScene()
    {
        SceneManager.LoadScene(2);
    }

    public void SwitchSceneToLeaderboard()
    {
        SceneManager.LoadScene(3);
    }


}
