using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{




    public void OnHover()
    {
        Debug.Log("Object inside cage");

        //Look through all children and stre any Mesh Renderers in the meshRenderer array.
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        //Use a looop to iterate through the meshRenderers array.
        foreach (MeshRenderer renderer in meshRenderers)
        {
            //Enable emission property of each renderer's material.
            renderer.material.EnableKeyword("_EMISSION");
        }
    }

    public void ExitHover()
    {

        Debug.Log("Object Not inside cage");
        //Look through all children and stre any Mesh Renderers in the meshRenderer array.
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        //Use a looop to iterate through the meshRenderers array.
        foreach (MeshRenderer renderer in meshRenderers)
        {
            //Disable emission property of each renderer's material.
            renderer.material.DisableKeyword("_EMISSION");
        }
    }

    public void playSound()
    {
        Debug.Log("sound played");
    }

    public void stopplaySound()
    {
        Debug.Log("stop sound played");
    }


    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SwitchSceneToGame()
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
