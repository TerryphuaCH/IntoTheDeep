using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SimpleGamePlayer
{
    public string userName;
    public string passWord;
    public string email;
    public long lastLoggedIn;
    public long createdOn;
    public long updatedOn;


    //empty constructor
    public SimpleGamePlayer()
    {

    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="passWord"></param>
    /// <param name="email"></param>

    public SimpleGamePlayer(string userName, string passWord, string email)
    {
        this.userName = userName;
        this.passWord = passWord;
        this.email = email;


        //timestamp properties
        var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        this.lastLoggedIn = timestamp;
        this.createdOn = timestamp;
        this.updatedOn = timestamp;
    }

    //helepr function
    public string SimpleGamePlayerToJson()
    {
        return JsonUtility.ToJson(this);
    }

    //helper function to print User details
    public string PrintPlayer()
    {
        return string.Format("User details: {0} \n Username: {1} \n  Password:  {2} \n Email:", this.userName, this.passWord, this.email);
    }
}
