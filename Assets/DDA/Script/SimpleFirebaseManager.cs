using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;

public class SimpleFirebaseManager : MonoBehaviour
{
    DatabaseReference dbPlayerStatsReference;
    DatabaseReference dbLeaderboardsReference;

    public void Awake()
    {
        InitializeFirebase();
    }

    public void InitializeFirebase()
    {
        dbPlayerStatsReference = FirebaseDatabase.DefaultInstance.GetReference("playerStats");
        dbLeaderboardsReference = FirebaseDatabase.DefaultInstance.GetReference("leaderboards");
    }

    /// <summary>
    /// Create a new entry ONLY if its the first time playing
    /// Update when there's exisiting entries
    /// </summary>
    /// <param name="uuid"></param>
    /// <param name="highScore"></param>
    /// <param name="time"></param>
    /// 
    /// <param name="userName"></param>
    public void UpdatePlayerStats(string uuid, int highScore, int time, string userName)
    {
        Query playerQuery = dbPlayerStatsReference.Child(uuid);

        //READ the data first and check whether there has been an enry based on my uuid
        playerQuery.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Sorry, there was an error creating ur entries, ERROR " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot playerStats = task.Result;
                //check if there is an entry created
                if (playerStats.Exists)
                {
                    //Update player stats
                    //compare exisiting highscore and set new highscore
                    //add time spent -> this is dependent on your own timer

                    //create a temp object sp which storeinfo from player stats
                    SimplePlayerStats sp = JsonUtility.FromJson<SimplePlayerStats>(playerStats.GetRawJsonValue());
                    sp.highScore += highScore;
                    sp.totalTimeSpent += time;
                    sp.updatedOn = sp.GetTimeUnix();

                    sp.highScore = highScore;
                    sp.totalTimeSpent = time;
                    


                    sp.highScore = highScore;
                    UpdatePlayerLeaderBoardEntry(uuid, sp.highScore, sp.updatedOn);
                    

                    //update with entire temp sp object
                    //path: playerstats/$uuid
                    dbPlayerStatsReference.Child(uuid).SetRawJsonValueAsync(sp.SimplePlayerStatsToJson());
                }
                else
                {
                    //CREATE Player stats
                    //if there;s no exisiting data, it is our first time player
                    SimplePlayerStats sp = new SimplePlayerStats(uuid, highScore, time, userName);

                    SimpleLeaderBoard lb = new SimpleLeaderBoard(userName, highScore);

                    //create new entries into firebase
                    dbPlayerStatsReference.Child(uuid).SetRawJsonValueAsync(sp.SimplePlayerStatsToJson());
                    dbLeaderboardsReference.Child(uuid).SetRawJsonValueAsync(lb.SimpleLeaderBoardToJson());
                }

            }
        });

    }

    public void UpdatePlayerLeaderBoardEntry(string uuid, int highScore, long updatedOn)
    {
        //rewatch the video on single entries
        //update only specific properties that we want

        //leaderboard/$
        dbLeaderboardsReference.Child(uuid).Child("highScore").SetValueAsync(highScore);
        dbLeaderboardsReference.Child(uuid).Child("updatedOn").SetValueAsync(updatedOn);

    }

    public async Task<List<SimpleLeaderBoard>> GetLeaderboard(int limit = 10)
    {

        Query q = dbLeaderboardsReference.OrderByChild("highScore").LimitToLast(limit);

        List<SimpleLeaderBoard> leaderBoardList = new List<SimpleLeaderBoard>();

        await q.GetValueAsync().ContinueWithOnMainThread(task =>
        {

            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Sorry, there was an error gettin leaderboard entries, : ERROR: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot ds = task.Result;

                if (ds.Exists)
                {

                    int rankCounter = 1;
                    foreach (DataSnapshot d in ds.Children)
                    {
                        //create temp objects based on the results
                        SimpleLeaderBoard lb = JsonUtility.FromJson<SimpleLeaderBoard>(d.GetRawJsonValue());

                        //add item to list
                        leaderBoardList.Add(lb);
                        Debug.LogFormat("Leaderboard: Rank {0} Playername {1} highScore{2}", rankCounter, lb.userName, lb.highScore);

                        //Debug.LogFormat("Leaderboard: Rank {0} Playername {1} High Score{2}", 
                        //rankCounter, lb.userName, lb.highScore);
                    }

                    //change list to descending order.
                    leaderBoardList.Reverse();

                    //for each simpleleaderboard object inside our leadboardlist
                    foreach (SimpleLeaderBoard lb in leaderBoardList)
                    {
                        Debug.LogFormat("Leaderboard: Rank {0} Playername {1} High Score{2}",
                            rankCounter, lb.userName, lb.highScore);

                        rankCounter++;
                    }
                }
            }
        });

        return leaderBoardList;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    public async Task<SimplePlayerStats> GetPlayerStats(string uuid)
    {
        Query q = dbPlayerStatsReference.Child(uuid).LimitToFirst(1);
        SimplePlayerStats playerStats = null;

        await dbPlayerStatsReference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Sorry, there was an error retreiving player stats : ERROR: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot ds = task.Result;//path -> playerstats/$uuid
                //path to the datasnapshot playerststa/$uuid/<we want this values>
                playerStats = JsonUtility.FromJson<SimplePlayerStats>(ds.Child(uuid).GetRawJsonValue());

                Debug.Log("ds... : " + ds.GetRawJsonValue());
                Debug.Log("player stats values... " + playerStats.SimplePlayerStatsToJson());
            }
        });

        return playerStats;
    }

    public void DeletePlayerStats(string uuid)
    {
        dbPlayerStatsReference.Child(uuid).RemoveValueAsync();
        dbLeaderboardsReference.Child(uuid).RemoveValueAsync();
    }

}
