using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Collectible : MonoBehaviour
{
    // function that is called when collectible is picked up
    void PickUpCollectible()
    {
        // play a sound effect
        // add collectible to player's inventory
        // destroy the collectible object
    }

    // function that is called when player interacts with collectible
    void InteractWithCollectible()
    {
        // play a sound effect
        // display a text prompt to the player
    }

    // detect when player's hand collides with collectible
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerHand")
        {
            PickUpCollectible();
        }
    }

    // check if player is interacting with collectible and call appropriate function
    void Update()
    {
        if (IsInteractingWithCollectible())
        {
            InteractWithCollectible();
        }
    }

    // define the IsInteractingWithCollectible function
    bool IsInteractingWithCollectible()
    {
        // define the playerIsInteractingWithCollectible variable
        bool playerIsInteractingWithCollectible = false;

        // add code here to check if player is interacting with collectible
        if (playerIsInteractingWithCollectible)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

