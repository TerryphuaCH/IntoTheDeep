using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject LoginUI;
    public GameObject RegisterUI;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

    }
    //Functions to change the login screen UI
    public void LoginScreen() //Back button
    {
        LoginUI.SetActive(true);
        RegisterUI.SetActive(false);

    }
    public void RegisterScreen() // Register button
    {
        LoginUI.SetActive(false);
        RegisterUI.SetActive(true);
    }

}

