using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SnapAndContinue : MonoBehaviour
{
    private bool isRotateDone = false;
    
    private float snapThreshold = 30;

    public Material correctMaterial;
    [FormerlySerializedAs("AudioSource")] public AudioSource audioSource;

    public GameLogic gameLogic;
    
    // Update is called once per frame
    void Update()
    {
        if (!isRotateDone)
        {
          CheckForSnapping();
        }
        else
        {
            gameLogic.GetComponent<GameLogic>().taskFinished = true;
            isRotateDone = false;
        }
        
    }

    void CheckForSnapping()
    {
        if (RotationAngleHelper.IsRotationWithinLimits(transform.localEulerAngles,10, gameLogic.objectOffset))
        {
            isRotateDone = true;
            transform.rotation = Quaternion.identity;

            GivePositiveFeedback();
        }
    }

    void GivePositiveFeedback()
    {
        transform.Find("Cube").gameObject.GetComponent<Renderer>().material = correctMaterial;
        audioSource.Play();
    }
    
}
