using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class sharkCage : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void sharktext()
    {
        text.text = "Task Completed, the shark will be transported to a medical facility.";
    }
}
