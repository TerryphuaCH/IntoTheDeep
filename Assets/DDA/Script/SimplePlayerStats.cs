using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SimplePlayerStats
{
    /*
       * key: uuid
         userName
         highScore
      */

    public string userName;
    public int totalTimeSpent;
    public int highScore;
    public long updatedOn;
    public long createdOn;
    public string uuid;

    //simple constructor
    public SimplePlayerStats()
    {

    }

    public SimplePlayerStats(string uuid, int highScore, int totalTimeSpent, string userName)
    {
        this.userName = userName;
        this.highScore = highScore;
        this.totalTimeSpent = totalTimeSpent;
 

        var timestamp = this.GetTimeUnix();
        this.updatedOn = timestamp;
        this.createdOn = timestamp;
    }

    public long GetTimeUnix()
    {
        return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();//1....
    }

    public string SimplePlayerStatsToJson()
    {
        return JsonUtility.ToJson(this);
    }

}
