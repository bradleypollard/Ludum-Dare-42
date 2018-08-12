using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WireVisualManager : MonoBehaviour
{
    WireManager wireManager;
    GridManager gridManager;
    VisualGridManager visualGridManager;

    public GameObject wirePrefab, wireParent, gridParent;

    private bool isShowingLocalWire = false;
    private Dictionary<CellCoordinates, List<GameObject>> completedWires;

    private List<CellCoordinates> localWire;
    private List<GameObject> localGameObjects;

	// Use this for initialization
	void Start ()
    {
        wireManager = FindObjectOfType<WireManager>();
        visualGridManager = FindObjectOfType<VisualGridManager>();
        gridManager = FindObjectOfType<GridManager>();
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
                Reset();
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

	public void Initialise()
	{
		completedWires = new Dictionary<CellCoordinates, List<GameObject>>();
	}

	public Dictionary<CellCoordinates, List<GameObject>> GetCompletedWires()
	{
		return completedWires;
	}

    public void ColourWires(Color _colour)
    {
        foreach (KeyValuePair<CellCoordinates, List<GameObject>> rows in completedWires)
        {
            foreach (GameObject wire in rows.Value)
            {
                wire.GetComponent<Image>().color = _colour;

                foreach (Image child in wire.GetComponentsInChildren<Image>())
                {
                    child.color = _colour;
                }
            }
        }
    }


    public void Reset()
    {
        bool fail = true;

        //Clear Local Wire
        if (localWire.Count >= 3)
        {
            GridObject endPoint = gridManager.GetCell(localWire[localWire.Count - 1]);
            if (endPoint != null)
            {
                GridObjectType endPointType = endPoint.ObjectType;
                if (endPointType == GridObjectType.Gate || endPointType == GridObjectType.Output)
                {
					foreach ( GameObject o in localGameObjects )
					{
						Wire_Visualiser v = o.GetComponent<Wire_Visualiser>();
						v.StartPulsing();
					}
                    completedWires.Add(localWire[1], localGameObjects);
                    fail = false;
                }
            }
        }

        if (fail)
        {
            ClearLocalPath();
        }

        localWire = null;
        localGameObjects = null;
        isShowingLocalWire = false;
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
                        currentWire.GetComponent<RectTransform>().sizeDelta = new Vector2((visualGridManager.GetScreenFromGrid(currentStart) - visualGridManager.GetScreenFromGrid(currentEnd)).magnitude + 5, 10.0f);
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
        GameObject currentWire = Instantiate(wirePrefab, gridParent.transform);
        Debug.Log("Visual Wire Manager:Creating Wire at Start point:" + _currentStart + " & End point:" + _currentEnd);
        currentWire.transform.SetAsFirstSibling();
        currentWire.GetComponent<RectTransform>().anchoredPosition = visualGridManager.GetScreenFromGrid(_currentStart) + new Vector2(-5, 0);
        currentWire.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 90) * Quaternion.LookRotation(Vector3.forward, new Vector3(_direction.x, _direction.y, 0.0f));
        currentWire.GetComponent<RectTransform>().sizeDelta = new Vector2((visualGridManager.GetScreenFromGrid(_currentStart) - visualGridManager.GetScreenFromGrid(_currentEnd)).magnitude + 5, 10.0f);
        currentWire.transform.SetParent(wireParent.transform);

        return currentWire;
    }

    public void CreateWireAndLink(CellCoordinates _currentStart, CellCoordinates _currentEnd, bool _useEnd)
    {
        Vector2Int direction = new Vector2Int((int)_currentEnd.X - (int)_currentStart.X, (int)_currentEnd.Y - (int)_currentStart.Y);
        GameObject wire = CreateWire(_currentStart, _currentEnd, direction);

        completedWires.Add(_useEnd ? _currentEnd : _currentStart, new List<GameObject>() { wire });
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

    public void ClearWire(CellCoordinates _cellCoordinates)
    {
        if(completedWires.ContainsKey(_cellCoordinates))
        {
            foreach(GameObject gameObjectToKill in completedWires[_cellCoordinates])
            {
                Destroy(gameObjectToKill);
            }
            completedWires.Remove(_cellCoordinates);
        }

        GridObject gridPoint = gridManager.GetCell(_cellCoordinates);
        
        if(gridPoint != null)
        {
            foreach (CellCoordinates connection in gridPoint.Coordinates)
            {
                if (completedWires.ContainsKey(connection))
                {
                    foreach (GameObject gameObjectToKill in completedWires[connection])
                    {
                        Destroy(gameObjectToKill);
                    }
                    completedWires.Remove(connection);
                    return;
                }
            }
        }
    }
}
