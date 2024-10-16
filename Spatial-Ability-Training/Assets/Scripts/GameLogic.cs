using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TransformGizmos;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GameLogic : MonoBehaviour
{

    public Material reference;
    public Material idle;
    public Material correct;
    public Material hover;

    public AudioSource audioSource;
    
    private float cubeScaleFactor = 0.3f;

    public List<GameObject> cubePrefabs;

    private SortedDictionary<string, List<int>> playedAngles;

    public GameObject interactabelParent;
    public GameObject referenceParent;

    [HideInInspector] public bool taskFinished = false;

    public GameObject gizmo;

    public TextMeshProUGUI doneTasksText;
    
    public TextMeshProUGUI totalTasksText;
    
    public GameObject tutorialText;

    public int objectOffset;
    public float gizmoOffset;
    
    private Quaternion defaultReferenceRotation;

    private GameObject currentInteractable;

    private TaskLogManager taskLogManager;

    private bool isTutorial = true;

    private int tutorialLength = 6;

    public GameObject goal;
    public GameObject rotateHere;

    private bool isGameRunning = true;
    
    // Start is called before the first frame update
    void Start()
    {
        taskLogManager = new TaskLogManager();
        Cursor.lockState = CursorLockMode.Confined;
        defaultReferenceRotation = Quaternion.Euler(0,0,0);
        playedAngles = new SortedDictionary<string, List<int>>();

        if (isTutorial)
        {
            totalTasksText.text = "/6";
            goal.SetActive(true);
            rotateHere.SetActive(true);
        }
        InitalizeTask();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameRunning)
        {
            if (taskFinished)
            {
                int finishedTasks = CountFinishedTasks();
                taskLogManager.FinishTask();
                taskFinished = false;
                ResetGizmo();
                if (isTutorial)
                {
                    doneTasksText.text = "Tutorial: " + finishedTasks;
                    if (finishedTasks >= tutorialLength)
                    {
                        tutorialText.SetActive(false);
                        isTutorial = false;
                        doneTasksText.text = "0";
                        totalTasksText.text = "/90";
                    }
                }
                else
                {
                    doneTasksText.text = (finishedTasks-tutorialLength).ToString();
                }
                StartCoroutine(WaitAndInitializeTask(1f));

            }
        }
        else
        {
            gizmo.SetActive(false);
            interactabelParent.SetActive(false);
            referenceParent.SetActive(false);
            tutorialText.SetActive(true);
            tutorialText.GetComponent<TextMeshProUGUI>().text =
                "Super, du hast alle Aufgaben bearbeitet! Melde dich jetzt bei der Versuchsleitung.";
        }
        
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            taskLogManager.SaveLogsToFile();
        }
    }
    
    private void OnApplicationQuit()
    {
        taskLogManager.SaveLogsToFile();
    }

    void InitalizeTask()
    {
        if (isGameRunning)
        {
            int angle = -1;
            GameObject cube = SelectCube();

            while (angle == -1)
            {
                if (!(cube is null))
                {
                    angle = SelectAngle(cube.name);
                    if (angle == -1)
                    {
                        cubePrefabs.Remove(cube);
                        cube = SelectCube();
                    }
                }
                else
                {
                    return;
                }
            }
            //Make sure no cubes remain in scene
            DestroyAllChildren(referenceParent);
            DestroyAllChildren(interactabelParent);

            //Instantiate & Initialize new Cubes
            GameObject referenceInstance = Instantiate(cube, Vector3.zero, defaultReferenceRotation);
            GameObject interactableInstance = Instantiate(cube, Vector3.zero, Quaternion.Euler(angle, angle + objectOffset, 0));
            currentInteractable = interactableInstance;
            InitializeReference(referenceInstance);
            InitializeInteractableGizmo(interactableInstance);

            InitGizmo(interactableInstance);

            taskLogManager.StartTask(cube.name, angle);  
        }
    }

    GameObject SelectCube()
    {
        //Test
        /*if (CountFinishedTasks() == 9)
        {
            isGameRunning = false;
            return null;
        }*/
        if (cubePrefabs.Count > 0)
        {
            // Select a random index between 0 and the length of the list (exclusive)
            int randomIndex = Random.Range(0, cubePrefabs.Count);

            // Retrieve the GameObject at the random index
            return cubePrefabs[randomIndex];
            
        }
        else
        {
            isGameRunning = false;
            Debug.LogWarning("The list of game objects is empty.");
            return null;
        }
    }

    int SelectAngle(string objName)
    {
        //Only Allow these three angles and shuffle them, so we do not always have the same difficulty at start
        List<int> allowedAnglesList = new (){50,100,150};
        allowedAnglesList = allowedAnglesList.OrderBy(x=> Random.Range(0,3)).ToList();
        
        //Add to a datatype that allows fetching and removing of element, then read it
        var queue = new Queue<int>(allowedAnglesList);
        var angle = queue.Dequeue();

        List<int> playedAngledForObj;

        //Check if for the given object one angle has already been played to avoid duplicates
        if (playedAngles.ContainsKey(objName) && playedAngles.TryGetValue(objName, out playedAngledForObj))
        {
            //If the selected angle was already used pick next one
            if (playedAngledForObj.Contains(angle))
            {
                angle = queue.Dequeue();

                //Again, If the selected angle was already used pick next one
                if (playedAngledForObj.Contains(angle))
                {
                    angle = queue.Dequeue();
                    //If this angle too was used, there are none left and we should not show this object anymore
                    if (playedAngledForObj.Contains(angle))
                    {
                        return -1;
                    }
                }
            }
            //We add this new angle to the already used ones
            playedAngledForObj.Add(angle);
            playedAngles[objName] = playedAngledForObj;
        }
        //If there is no entry, we add the first with the used angle
        else
        {
            playedAngles.Add(objName,new List<int>(){angle});
        }
        
        return angle;
    }

    private void InitializeReference(GameObject referenceInstance)
    {
        referenceInstance.transform.parent = referenceParent.transform;
        
        //Set correct position, rotation and color
        referenceInstance.transform.localScale = new Vector3(cubeScaleFactor, cubeScaleFactor, cubeScaleFactor); // change its local scale in x y z format
        referenceInstance.transform.localPosition = Vector3.zero;
        var referenceCube = referenceInstance.transform.Find("Cube");
        referenceCube.GetComponent<Renderer>().material = reference;
    }

    private void InitializeInteractableGizmo(GameObject interactableInstance)
    {
        interactableInstance.transform.parent = interactabelParent.transform;
        
        //Set correct position, rotation and color
        interactableInstance.transform.localScale = new Vector3(cubeScaleFactor, cubeScaleFactor, cubeScaleFactor);
        interactableInstance.transform.localPosition = Vector3.zero;
        var interactableCube = interactableInstance.transform.Find("Cube");
        interactableCube.GetComponent<Renderer>().material = idle;
        
        //Add needed script
        interactableInstance.AddComponent<SnapAndContinue>();

        //Add variables for snapping
        SnapAndContinue snapScript = interactableInstance.GetComponent<SnapAndContinue>();
        snapScript.audioSource = audioSource;
        snapScript.correctMaterial = correct;
        snapScript.gameLogic = this;

        interactableCube.GetComponent<MeshCollider>().enabled = false;
    }
    private void InitializeInteractableDrag(GameObject interactableInstance)
    {
        interactableInstance.transform.parent = interactabelParent.transform;
        
        //Set correct position, rotation and color
        interactableInstance.transform.localScale = new Vector3(cubeScaleFactor, cubeScaleFactor, cubeScaleFactor);
        interactableInstance.transform.localPosition = Vector3.zero;
        var interactableCube = interactableInstance.transform.Find("Cube");
        interactableCube.GetComponent<Renderer>().material = idle;
        
        //Add needed scripts
        interactableInstance.AddComponent<RotateOnMouseDrag>();
        interactableCube.AddComponent<ColorOnHover>();
        interactableInstance.AddComponent<SnapAndContinue>();

        //Add variables for Rotate Script
        RotateOnMouseDrag rotateScript = interactableInstance.GetComponent<RotateOnMouseDrag>();
        rotateScript.idleMaterial = idle;
        rotateScript.gameLogic = this;
        
        //Add variables for snapping
        SnapAndContinue snapScript = interactableInstance.GetComponent<SnapAndContinue>();
        snapScript.audioSource = audioSource;
        snapScript.correctMaterial = correct;
        snapScript.gameLogic = this;

        //Add variables for Color Script
        ColorOnHover colorScript = interactableCube.GetComponent<ColorOnHover>();
        colorScript.idleMaterial = idle;
        colorScript.hoverMaterial = hover;
    }

    private void DestroyAllChildren(GameObject parent)
    {
        foreach(Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private int CountFinishedTasks()
    {
        int doneTasks = 0;
        foreach (var (key, value) in playedAngles)
        {
            doneTasks += value.Count;
        }

        return doneTasks;
    }

    private void InitGizmo(GameObject interactableInstance)
    {
        gizmo.GetComponent<GizmoController>().m_targetObject = interactableInstance;
        gizmo.GetComponent<GizmoController>().Init(gizmoOffset);
    }

    private void ResetGizmo()
    {
        gizmo.GetComponent<GizmoController>().m_rotation.MouseUpCode(0);
        gizmo.GetComponent<GizmoController>().m_rotation.MouseUpCode(1);
        gizmo.GetComponent<GizmoController>().m_rotation.MouseUpCode(2);
        gizmo.GetComponent<GizmoController>().m_rotation.isDragAllowed = false; 
    }
    
    private IEnumerator WaitAndInitializeTask(float delay)
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(delay);
        
        if (isTutorial)
        {
            int finishedTasks = CountFinishedTasks();
            UpdateTutorial(finishedTasks);
        }

        // Now that the delay is over, initialize the next task
        InitalizeTask();
        currentInteractable.GetComponent<SnapAndContinue>().Reset();
    }

    private void UpdateTutorial(int tasksDone)
    {
        TextMeshProUGUI text = tutorialText.GetComponent<TextMeshProUGUI>();
        switch (tasksDone)
        {
            case 1:
                goal.SetActive(false);
                rotateHere.SetActive(false);
                text.text = "Das klappt schon super! Sobald das Objekt in einem gewissen Abstand zum richtigen Winkel ist, rastet es automatisch ein. Achte mal darauf!";
                break;
            case 2:
                text.text = "Super! Jeder der drei Kreise steuert eine Achse separat. Es gibt also viele verschiedene Wege, um das Objekt richtig zu drehen. Teste es selbst!";
                break;
            case 3:
                text.text = "Das war klasse! Die zu drehenden Objekte und deren Winkel wechsel mit jeder Aufgabe. Versuche es nochmal!";

                break;
            case 4:
                text.text = "Gut gemacht! Oben rechts im Eck findest du den aktuellen Fortschritt der Aufgaben, so hast du diesen immer im Blick. Sieh selbst, wie er nach dieser Aufgabe hochzählt!";

                break;
            case 5:
                text.text = "Toll! Nach dieser Aufgabe starten nun das Spiel, viel Erfolg!";
                break;
            default:
                break;
        }
    }
}
