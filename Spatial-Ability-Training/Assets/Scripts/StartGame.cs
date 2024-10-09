using System;
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

    private Transform example;

    private bool rotate = false;

    private void Start()
    {
        example = interactableParent.transform.Find("RotateObj");
    }

    public void initiateExample()
    {
        transform.Find("Button").gameObject.SetActive(false);
        tutorialText.SetActive(true);
        interactableParent.SetActive(true);
        referenceParent.SetActive(true);
        rotate = true;
    }
    private void Update()
    {
        if (rotate)
        {
            Debug.Log("Rotate True");
            Debug.Log("Current Rotate: " +  example.gameObject.transform.rotation);
            // Rotate towards the target rotation at a constant speed (degrees per second)
            example.gameObject.transform.rotation = Quaternion.RotateTowards(
                example.gameObject.transform.rotation,   // Current rotation
                Quaternion.Euler(0,35,0),       // Target rotation
                25 * Time.deltaTime // Rotation step per frame
            );

            // Optional: Stop the rotation if we've reached the target
            if (example.gameObject.transform.rotation == Quaternion.Euler(0,35,0))
            {
                Debug.Log("Rotation completed");
                rotate = false;
                initiateGame();
            }  
        }
        
    }
    
    
    public void initiateGame()
    {
        tutorialText.GetComponent<TextMeshProUGUI>().text =
            "Nutze dafür die Kreise um das Objekt, um es an einer der drei Achsen zu drehen. Probiere es selbst!";
        counter.SetActive(true);
        gameLogic.SetActive(true);
        gizmo.SetActive(true);
    }
    
    //Idee: Sample Objekte nehmen, das rechte dann Skriptseitig rotieren,
    //und so demonstrieren was gemeint ist. Am besten im Abstimmung mit erscheinendem Text
    //Dann erst eigentliches Spiel laden (alles aktivierne), und Text in die Richtung "nutze dieses Tool dafür"

}
