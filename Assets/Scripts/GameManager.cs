using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //public AuthManager auth;
    public GameObject panel;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] float duration, currentTime;
    public TextMeshProUGUI ClicksTotalText;
    int TotalClicks;

    //public SimpleFirebaseManager firebaseMrg;
    //public bool isPlayerStatUpdated;

    public void Start()
    {
        //isPlayerStatUpdated = false;
        panel.SetActive(false);
        currentTime = duration;
        timeText.text = currentTime.ToString();
        StartCoroutine(TimeIEn());
    }

    IEnumerator TimeIEn()
    {
        while (currentTime >= 0)
        {
            timeText.text = currentTime.ToString();
            yield return new WaitForSeconds(1f);
            currentTime--;
        }
        //OpenPanel();
    }

    //void OpenPanel()
   // {
      //  if (!isPlayerStatUpdated)
    //    {
    //        UpdatePlayerStat(this.TotalClicks);//
    //    }
   //     isPlayerStatUpdated = true;
    //    timeText.text = "";
   //     panel.SetActive(true);
  //  }

    //public void UpdatePlayerStat(int score)
    //{
       // firebaseMrg.UpdatePlayerStats(auth.GetCurrentUser().UserId, score, auth.GetCurentUserDisplayName());
    //}


    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddClicks()
    {
        TotalClicks++;
        ClicksTotalText.text = TotalClicks.ToString("0");
    }

    public void SwitchSceneToMain()
    {
        SceneManager.LoadScene(1);
    }

    public void SwitchSceneToPlayerStats()
    {
        SceneManager.LoadScene(4);
    }

    public void SwitchSceneToLeaderboard()
    {
        SceneManager.LoadScene(3);
    }
}
