using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnMouseDrag : MonoBehaviour
{

    private Transform mainCameraTranform;

    private Vector3 mousePrevPos = Vector3.zero;

    private Vector3 mousePosDelta = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        mainCameraTranform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            mousePosDelta = Input.mousePosition - mousePrevPos;

            transform.Rotate(transform.up, Vector3.Dot(mousePosDelta, mainCameraTranform.right), Space.World);
        }
        
        mousePrevPos = Input.mousePosition;
    }
}
