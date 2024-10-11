using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public GameObject gameLogic;
    public GameObject gizmo;
    public GameObject counter;
    public GameObject interactableParent;
    public GameObject referenceParent;
    public GameObject tutorialText;
    public GameObject decorations;
    public GameObject logo;

    private Transform example;

    private bool rotate = false;

    private void Start()
    {
        example = interactableParent.transform.Find("RotateObj");
    }

    public void initiateExample()
    {
        transform.Find("Button").gameObject.SetActive(false);
        decorations.gameObject.SetActive(false);
        logo.gameObject.SetActive(false);
        
        tutorialText.SetActive(true);
        interactableParent.SetActive(true);
        referenceParent.SetActive(true);
        StartCoroutine(WaitAndStartExample(4f));
    }
    private void Update()
    {
        if (rotate)
        {
            // Rotate towards the target rotation at a constant speed (degrees per second)
            example.gameObject.transform.rotation = Quaternion.RotateTowards(
                example.gameObject.transform.rotation,   // Current rotation
                Quaternion.Euler(0,35,0),       // Target rotation
                25 * Time.deltaTime // Rotation step per frame
            );

            // Optional: Stop the rotation if we've reached the target
            if (example.gameObject.transform.rotation == Quaternion.Euler(0,35,0))
            {
                rotate = false;
                StartCoroutine(WaitAndInitializeGame(2f));
            }  
        }
        
    }
    
    private IEnumerator WaitAndInitializeGame(float delay)
    {
        yield return new WaitForSeconds(delay);
        InitiateGame();
    }
    
    private IEnumerator WaitAndStartExample(float delay)
    {
        yield return new WaitForSeconds(delay);
        rotate = true;
    }


    public void InitiateGame()
    {
        tutorialText.GetComponent<TextMeshProUGUI>().text =
            "Klicke und ziehe die Kreise um das Objekt, um es an einer der drei Achsen zu drehen. Probiere es selbst und drehe das Objekt, sodass es genau wie das Linke gedreht ist!";
        counter.SetActive(true);
        gameLogic.SetActive(true);
        gizmo.SetActive(true);
    }
}
