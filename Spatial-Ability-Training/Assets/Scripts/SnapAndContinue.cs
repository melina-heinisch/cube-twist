using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class SnapAndContinue : MonoBehaviour
{
    private bool isRotateDone = false;
    
    private float snapThreshold = 30;

    public Material correctMaterial;
    [FormerlySerializedAs("AudioSource")] public AudioSource audioSource;

    public GameLogic gameLogic;

    private int SsnapGraceRange = 16;
    
    // Update is called once per frame
    void Update()
    {
        if (!isRotateDone)
        {
          CheckForSnapping();
        }

    }

    void CheckForSnapping()
    {
        if (!isRotateDone)
        {
             if (RotationAngleHelper.IsRotationWithinLimits(transform.localEulerAngles,SsnapGraceRange/2, gameLogic.objectOffset))
             {
                 isRotateDone = true;
                 if (gameLogic.isActiveAndEnabled)
                 {
                     gameLogic.GetComponent<GameLogic>().taskFinished = true; 
                 }
                 transform.rotation = Quaternion.Euler(0,gameLogic.objectOffset,0);
     
                 GivePositiveFeedback();
             }   
        }
        
    }

    void GivePositiveFeedback()
    {
        transform.Find("Cube").gameObject.GetComponent<Renderer>().material = correctMaterial;
        audioSource.Play();
    }

    public void Reset()
    {
        isRotateDone = false;
    }
}
