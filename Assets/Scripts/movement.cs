using UnityEngine;
using TMPro;

public class movement : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    public TMP_Text textMesh;

    void Update()
    {
        // Use Mathf.PingPong to move the text mesh up and down
        float yPos = Mathf.PingPong(Time.time * movementSpeed, movementSpeed);
        textMesh.transform.Translate(0.0f, yPos, 0.0f);
    }
}
