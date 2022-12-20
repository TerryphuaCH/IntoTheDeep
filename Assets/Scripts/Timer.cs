using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{

    public AuthManager auth;
    public GameObject panel;

    public TMP_Text scoreText;

    public int rubbishNumber;
    public TextMeshProUGUI ScoreText;

    public Text word;
    public TextMeshProUGUI TrapText;
    

    public SimpleFirebaseManager firebaseMrg;
    public bool isPlayerStatUpdated;
    public string uuid;
    public string userName;
    public long updateOn;

    public TextMeshProUGUI timerMinutes;
    public TextMeshProUGUI timerSeconds;
    public TextMeshProUGUI timerSeconds100;

    private float startTime;
    private float stopTime;
    private float timerTime;
    private bool isRunning = false;

    // Use this for initialization


    public void TimerStart()
    {
        if (!isRunning)
        {
            print("START");
            isRunning = true;
            startTime = Time.time;
        }
    }



    public int score;
    public int time;

    public void TimerStop(int score, int time)
    {
        if (isRunning)
        {
            print("STOP");
            isRunning = false;
            stopTime = timerTime;

            // Call the UpdatePlayerStat method here, passing in the score as an argument
            UpdatePlayerStat(uuid,score, time,userName);

        }
    }

    public void uasdas()
    {
        TimerStop(score, time);
    }

    public void UpdatePlayerStat(string uuid,int score,int time,string userName )
    {
        firebaseMrg.UpdatePlayerStats(uuid,score,time,userName);
    }

    public void UpdateLeaderBoard(string uuid,int score,long updatedOn)
    {
        firebaseMrg.UpdatePlayerLeaderBoardEntry(uuid, score, updateOn);
    }

    public  void updateleader()
    {
        UpdateLeaderBoard(uuid,score,updateOn);
    }

    // Update is called once per frame
    void Update()
    {
        timerTime = stopTime + (Time.time - startTime);
        int minutesInt = (int)timerTime / 60;
        int secondsInt = (int)timerTime % 60;
        int seconds100Int = (int)(Mathf.Floor((timerTime - (secondsInt + minutesInt * 60)) * 100));

        if (isRunning)
        {
            timerMinutes.text = (minutesInt < 10) ? "0" + minutesInt : minutesInt.ToString();
            timerSeconds.text = (secondsInt < 10) ? "0" + secondsInt : secondsInt.ToString();
            timerSeconds100.text = (seconds100Int < 10) ? "0" + seconds100Int : seconds100Int.ToString();
        }
    }
}
