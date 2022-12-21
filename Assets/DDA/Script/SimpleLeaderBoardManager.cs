using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.SceneManagement;//to load back to main menu

public class SimpleLeaderBoardManager : MonoBehaviour
{
    public SimpleFirebaseManager fbManager;
    public GameObject rowPrefab;
    public Transform tableContent;

    void Start()
    {
        GetLeaderboard();
    }

    public void GetLeaderboard()
    {
        UpdateLeaderboardUI();
    }

    /// <summary>
    /// Get and update leaderboardUI
    /// </summary>
    public async void UpdateLeaderboardUI()
    {
        var leaderBoardList = await fbManager.GetLeaderboard(10);
        int rankCounter = 1;

        //clear all leaderboard entries in UI
        foreach (Transform item in tableContent)
        {
            Destroy(item.gameObject);
        }

        //create prefabs of our rows
        //assign each value from list to the prefab text content
        foreach (SimpleLeaderBoard lb in leaderBoardList)
        {
            Debug.LogFormat("Leaderboard Mgr: Rank {0} Playername{1} High Score{2}",
                                    rankCounter, lb.userName, lb.highScore);

            //create prefabs in the position of tableContent
            GameObject entry = Instantiate(rowPrefab, tableContent);
            TextMeshProUGUI[] leaderBoardDetails = entry.GetComponentsInChildren<TextMeshProUGUI>();
            leaderBoardDetails[0].text = rankCounter.ToString();
            leaderBoardDetails[1].text = lb.userName;
            leaderBoardDetails[2].text = lb.highScore.ToString();

            rankCounter++;
        }

    }

    public void SwitchSceneToMain()
    {
        SceneManager.LoadScene(1);
    }
}

