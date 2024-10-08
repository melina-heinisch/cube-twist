using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ColorOnHover : MonoBehaviour
{

    public Material hoverMaterial; 

    public Material idleMaterial;

    private void OnMouseEnter()
    {
        gameObject.GetComponent<Renderer>().material = hoverMaterial;
    }

    private void OnMouseExit()
    {
        if (!Input.GetMouseButton(0))
        { 
            gameObject.GetComponent<Renderer>().material = idleMaterial;
        }
    }
}
