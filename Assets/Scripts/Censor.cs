using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Censor : MonoBehaviour
{
    public GameManager GameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "rubbish")
        {
            GameManager.RubbishCounter();
        }
    }
}
