using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireVisualManager : MonoBehaviour
{
    WireManager wireManager;
    VisualGridManager visualGridManager;

    public GameObject wirePrefab, wireParent;

    private bool isShowingLocalWire = false;
    private Dictionary<CellCoordinates, List<GameObject>> completedWires;

    private List<CellCoordinates> localWire;
    private List<GameObject> localGameObjects;

	// Use this for initialization
	void Start ()
    {
        wireManager = FindObjectOfType<WireManager>();
        visualGridManager = FindObjectOfType<VisualGridManager>();

        completedWires = new Dictionary<CellCoordinates, List<GameObject>>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(wireManager.IsInWireEditMode())
        {
            if (!isShowingLocalWire)
            {
                //Start showing a local wire
                isShowingLocalWire = true;
            }
        }
        else
        {
            if (isShowingLocalWire)
            {
                //Clear Local Wire
                if (localWire.Count >= 2)
                {
                    completedWires.Add(localWire[0], localGameObjects);
                }

                localWire = null;
                localGameObjects = null;
                isShowingLocalWire = false;
            }
        }
        
        //Draw the Wire
        if(isShowingLocalWire)
        {
            List<CellCoordinates> wirePath = wireManager.GetCurrentPath();

            if (localWire == null || localWire.Count != wirePath.Count)
            {
                //We've changed somewhere                   
                GenerateWire(wirePath);
            }
            else
            {
                for (int i = 0; i < wirePath.Count; i++)
                {
                    //Check for changes
                    if (localWire[i].X != wirePath[i].X || localWire[i].Y != wirePath[i].Y)
                    {
                        //We've changed somewhere                   
                        GenerateWire(wirePath);
                    }
                }
            }

        }
	}

    void GenerateWire(List<CellCoordinates> _wirePath)
    {
        ClearLocalPath();

        bool newWire = true;
        CellCoordinates currentStart = new CellCoordinates(0,0), currentEnd = new CellCoordinates(0, 0);
        Vector2Int direction = Vector2Int.zero;
        GameObject currentWire = null;

        for (int i = 0; i < _wirePath.Count; i++)
        {
            if (newWire) //If this is a new wire, set this as the start point
            {
                currentStart = _wirePath[i];
                newWire = false;

                Debug.Log("Visual Wire Manager: Start line at " + currentStart);
            }
            else
            {
                //Have we picked a direction?
                if (direction == Vector2Int.zero)
                {
                    currentEnd = _wirePath[i];
                    direction = new Vector2Int((int)currentEnd.X - (int)currentStart.X, (int)currentEnd.Y - (int)currentStart.Y);
                    Debug.Log("Visual Wire Manager: Forming at direction " + direction +" at " + currentEnd);

                    currentWire = CreateWire(currentStart, currentEnd, direction);
                }
                else
                {
                    //Is this cell in the same direction
                    Vector2Int testDirection = new Vector2Int((int)_wirePath[i].X - (int)currentEnd.X, (int)_wirePath[i].Y - (int)currentEnd.Y);
                    if(testDirection == direction)
                    {
                        currentEnd = _wirePath[i];
                        currentWire.GetComponent<RectTransform>().sizeDelta = new Vector2((visualGridManager.GetScreenFromGrid(currentStart) - visualGridManager.GetScreenFromGrid(currentEnd)).magnitude, 10.0f);
                        Debug.Log("Visual Wire Manager: Direction continues at " + currentEnd);
                    }
                    else
                    {
                        Debug.Log("Visual Wire Manager: Direction changes at " + currentEnd);
                        //This is where this wire ends
                        localGameObjects.Add(currentWire);

                        //Reset and Continue
                        currentWire = null;
                        currentStart = currentEnd;
                        direction = Vector2Int.zero;
                        i--;
                    }                 
                }
            }
        }

        if(currentWire != null)
        {
            localGameObjects.Add(currentWire);
        }

        localWire = _wirePath;
    }

    GameObject CreateWire(CellCoordinates _currentStart, CellCoordinates _currentEnd, Vector2Int _direction)
    {
        GameObject currentWire = Instantiate(wirePrefab, wireParent.transform);
        Debug.Log("Visual Wire Manager:Creating Wire at Start point:" + _currentStart + " & End point:" + _currentEnd);
        currentWire.GetComponent<RectTransform>().anchoredPosition = visualGridManager.GetScreenFromGrid(_currentStart);
        currentWire.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 90) * Quaternion.LookRotation(Vector3.forward, new Vector3(_direction.x, _direction.y, 0.0f));
        currentWire.GetComponent<RectTransform>().sizeDelta = new Vector2((visualGridManager.GetScreenFromGrid(_currentStart) - visualGridManager.GetScreenFromGrid(_currentEnd)).magnitude, 10.0f);

        return currentWire;
    }

    void ClearLocalPath()
    {
        if (localGameObjects != null)
        {
            foreach (GameObject localGameObject in localGameObjects)
            {
                Destroy(localGameObject);
            }
        }

        localWire = new List<CellCoordinates>();
        localGameObjects = new List<GameObject>();
    }
}
