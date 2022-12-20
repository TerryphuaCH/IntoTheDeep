using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System;
using UnityEngine.SceneManagement;

public class SimplePlayerStatsManager : MonoBehaviour
{
    public TextMeshProUGUI playerTimeSpent;
    public TextMeshProUGUI playerHighScore;
    public TextMeshProUGUI playerLastPlayed;
    public TextMeshProUGUI playerName;

    public SimpleFirebaseManager fbMgr;
    public AuthManager auth;

    //Start is called b4 the first frame update
    void Start()
    {
        //empty ui in the start
        ResetStatsUI();


        //retrieve current logged in user's uuid
        //update ui
        UpdatePlayerStats(auth.GetCurrentUser().UserId);
    }

    public async void UpdatePlayerStats(string uuid)
    {
        SimplePlayerStats playerStats = await fbMgr.GetPlayerStats(uuid);

        if (playerStats != null)
        {
            Debug.Log("playerstats.... : " + playerStats.SimplePlayerStatsToJson());
            playerTimeSpent.text = playerStats.totalTimeSpent + " secs";
            playerHighScore.text = playerStats.highScore.ToString();
            playerLastPlayed.text = UnixToDateTime(playerStats.updatedOn);//ToString();
        }
        else
        {
            //reset our ui to 0 and NA
            ResetStatsUI();
        }
        playerName.text = auth.GetCurrentUserDisplayName();
    }

    public void ResetStatsUI()
    {
        playerTimeSpent.text = "0";
        playerHighScore.text = "0";
        playerLastPlayed.text = "0";
    }

    public void DeletePlayerStats()
    {
        fbMgr.DeletePlayerStats(auth.GetCurrentUser().UserId);

        //refresh my player stats on screen
        UpdatePlayerStats(auth.GetCurrentUser().UserId);
    }

    /// <summary>
    /// convert to a reliable date time value 3 nov 2021
    /// </summary>
    /// <param name="timestamp">in unix seconds</param>
    /// <returns></returns>
    public string UnixToDateTime(long timestamp)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);//number of secs from 1/1/1970
        DateTime dateTime = dateTimeOffset.LocalDateTime;//convert to current time format UTC+0 ... but in Singapore +8

        return dateTime.ToString("dd MMM yyyy");


    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(1);
    }

}
