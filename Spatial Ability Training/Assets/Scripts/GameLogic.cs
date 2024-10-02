using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameLogic : MonoBehaviour
{

    public Material reference;

    public Material idle;

    private float cubeScaleFactor = 0.3f;

    public List<GameObject> cubePrefabs;

    private SortedDictionary<string, List<int>> playedAngles;

    public GameObject interactabelParent;

    public GameObject referenceParent;

    public 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void initalizeTask()
    {
        int angle = -1;
        GameObject cube = selectCube();

        while (angle == -1)
        {
            angle = selectAngle(cube.name);
            if (angle == -1)
            {
                cubePrefabs.Remove(cube);
                cube = selectCube();
            }
        }
        
        //(Instantiate (cube, Vector3.zero, Vector3.zero) as GameObject).transform.parent = referenceParent.transform;
        //(Instantiate (cube, Vector3.zero, new Vector3(angle,angle,0)) as GameObject).transform.parent = interactabelParent.transform;
        
        
        
        

    }

    GameObject selectCube()
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

    int selectAngle(string objName)
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
}
