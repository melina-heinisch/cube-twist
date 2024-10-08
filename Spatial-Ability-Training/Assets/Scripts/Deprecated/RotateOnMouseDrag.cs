using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class RotateOnMouseDrag : MonoBehaviour
{

    private Camera mainCamera;

    private Vector3 mousePrevPos = Vector3.zero;
    private Vector3 mousePosDelta = Vector3.zero;
    
    private Ray ray;
    private RaycastHit hit;

    private bool isRotateAllowed = false;
    private bool isRotateDone = false;
    
    private float snapThreshold = 30;

    public Material idleMaterial;

    public GameLogic gameLogic;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRotateDone)
        {
          setIsRotateAllowed();
          rotateAroundClickOnMouseDrag();
          mousePrevPos = Input.mousePosition;  
        }
        else
        {
            gameLogic.GetComponent<GameLogic>().taskFinished = true;
        }
        
    }

    void setIsRotateAllowed()
    {
        // If left mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            // If mouse cursor is currently on a rotatable object
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Rotate"))
                {
                    isRotateAllowed = true;
                }
            }
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            isRotateAllowed = false;
            gameObject.GetComponent<Renderer>().material = idleMaterial;

        }
    }

    void rotateAroundCenterOnMouseDrag()
    {
        if (isRotateAllowed && Input.GetMouseButton(0))
        {
            mousePosDelta = Input.mousePosition - mousePrevPos;

            // Rotate around Object center
            transform.Rotate(Vector3.up, Vector3.Dot(mousePosDelta, Vector3.left), Space.World);
            transform.Rotate(Vector3.right,Vector3.Dot(mousePosDelta, Vector3.up), Space.World);
        }
    }

    void rotateAroundClickOnMouseDrag()
    {
        if (isRotateAllowed && Input.GetMouseButton(0))
        {
            mousePosDelta = Input.mousePosition - mousePrevPos;

            //Rotate around where you click
            transform.RotateAround(hit.transform.position, Vector3.up, Vector3.Dot(mousePosDelta, Vector3.left));
            transform.RotateAround(hit.transform.position, Vector3.right, Vector3.Dot(mousePosDelta, Vector3.up));
        }
    }

}
