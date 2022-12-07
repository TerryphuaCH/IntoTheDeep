using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableHighlight : MonoBehaviour
{
    // Start is called before the first frame update

    public void OnHover()
    {
        //Look through all children and stre any Mesh Renderers in the meshRenderer array.
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        //Use a looop to iterate through the meshRenderers array.
        foreach(MeshRenderer renderer in meshRenderers)
        {
            //Enable emission property of each renderer's material.
            renderer.material.EnableKeyword("_EMISSION");
        }
    }

    public void ExitHover()
    {
        //Look through all children and stre any Mesh Renderers in the meshRenderer array.
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        //Use a looop to iterate through the meshRenderers array.
        foreach (MeshRenderer renderer in meshRenderers)
        {
            //Disable emission property of each renderer's material.
            renderer.material.DisableKeyword("_EMISSION");
        }
    }

}
