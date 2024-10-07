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

    public GizmoController gizmo;

    public TextMeshProUGUI doneTasksText;

    // Start is called before the first frame update
    void Start()
    {
        playedAngles = new SortedDictionary<string, List<int>>();
        InitalizeTask();
    }

    // Update is called once per frame
    void Update()
    {
        if (taskFinished)
        {
            doneTasksText.text = CountFinishedTasks();
            ResetGizmo();
            InitalizeTask();
        }
    }

    void InitalizeTask()
    {
        taskFinished = false;
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
        }
        //Make sure no cubes remain in scene
        DestroyAllChildren(referenceParent);
        DestroyAllChildren(interactabelParent);

        //Instantiate & Initialize new Cubes
        var referenceInstance = Instantiate(cube, Vector3.zero, Quaternion.identity);
        var interactableInstance = Instantiate(cube, Vector3.zero, Quaternion.Euler(angle, angle, 0));
        InitializeReference(referenceInstance);
        InitializeInteractableGizmo(interactableInstance);
        
        InitGizmo(interactableInstance);
        
    }

    GameObject SelectCube()
    {
        if (cubePrefabs.Count > 0)
        {
            // Select a random index between 0 and the length of the list (exclusive)
            int randomIndex = Random.Range(0, cubePrefabs.Count);

            // Retrieve the GameObject at the random index
            return cubePrefabs[randomIndex];
            
        }
        else
        {
            //TODO: Exit Game
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

    private string CountFinishedTasks()
    {
        int doneTasks = 0;
        foreach (var (key, value) in playedAngles)
        {
            doneTasks += value.Count;
        }

        return doneTasks.ToString();
    }

    private void InitGizmo(GameObject interactableInstance)
    {
        gizmo.m_targetObject = interactableInstance;
        gizmo.Init();
    }

    private void ResetGizmo()
    {
        gizmo.m_rotation.MouseUpCode(0);
        gizmo.m_rotation.MouseUpCode(1);
        gizmo.m_rotation.MouseUpCode(2);
        gizmo.m_rotation.isDragAllowed = false; 
    }
}
