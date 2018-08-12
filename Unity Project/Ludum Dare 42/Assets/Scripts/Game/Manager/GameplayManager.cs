using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoboTools.Helpers;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    //Debug Variables
    public bool debug_StartGameOnLoad = false;
	public string debug_LevelToLoad = "";

    //Game Constants
    const float startTimeLeft = 100;

    //Game Variables
    private float timeLeft;
    private int score, wave, gridWidth, gridHeight;
    private bool isPlaying;

    private Dictionary<InputCell, GameObject> inputs;
    private Dictionary<OutputCell, GameObject> outputs;
    private Dictionary<GridObject, GameObject> placedGridObjects;

    //UI Accessors (Cause Lazy)
    public Text timerText;
    public Text waveText;
    public Text scoreText;

    public RectTransform gridParent, scrollViewParent;
    public GameObject inputCellPrefab, outputCellPrefab;

    //Components
    private GridManager gridManager;
    private WireVisualManager wireVisualManager;

    private readonly Color solvedColour = new Color(0.423f, 0.858f, 0.612f);
    private readonly Color unsolvedColour = new Color(0.925f, 0.941f, 0.945f);

	private Dictionary<string, LevelFile> m_levels;

    // Use this for initialization
    void Start ()
    {
		SetupStartingLevels();

        //Get Components
        gridManager = FindObjectOfType<GridManager>();
        wireVisualManager = FindObjectOfType<WireVisualManager>();

        //Do Debug Logic
        if (debug_StartGameOnLoad)
        {
            StartGame();
        }		
	}
	
	void StartGame()
    {
        //Reset Values
        timeLeft = startTimeLeft;
        score = 0;
        wave = -1;
        isPlaying = true;
        gridWidth = 5;
        gridHeight = 5;
        inputs = new Dictionary<InputCell, GameObject>();
        outputs = new Dictionary<OutputCell, GameObject>();
        placedGridObjects = new Dictionary<GridObject, GameObject>();

        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (isPlaying)
        {
            GenerateWave();

            while(isPlaying && !IsWaveBeaten())
            {
                timeLeft -= Time.deltaTime;

                if(timeLeft <= 0.0f)
                {
                    isPlaying = false;
                }

                //Update Outputs
                foreach(KeyValuePair<OutputCell, GameObject> output in outputs)
                {
                    foreach (Image image in output.Value.GetComponentsInChildren<Image>())
                    {
                        //Colour wires if complete
                        if (output.Key.IsCurrentlySatisfied())
                        {
                            image.color = solvedColour;
                        }
                        else
                        {
                            image.color = unsolvedColour;
                        }
                    }

                    if (output.Key.IsCurrentlySatisfied())
                    {
                        output.Value.GetComponentInChildren<Text>().color = solvedColour;
                    }
                    else
                    {
                        output.Value.GetComponentInChildren<Text>().color = unsolvedColour;
                    }                  
                }

                yield return null;
            }

            score += GetWaveClearScore();
            timeLeft += GetWaveClearTime();
        }
    }

    private void GenerateWave()
    {
        //Increment Wave
        wave++;

        if(wave == 0)
        {
			LevelFile file = null;
			if ( debug_LevelToLoad != "" )
			{
				file = m_levels[debug_LevelToLoad];
        }
			gridManager.Initialise( file );
		}
        else
        {
			bool canAdvance = gridManager.AdvanceGeneration(); // TODO: Use for victory screen
        }

        //Update Visual Grid
        VisualGridManager visualGridManager = FindObjectOfType<VisualGridManager>();
        visualGridManager.gridWidth = gridWidth;
        visualGridManager.gridHeight = gridHeight;

        //Update Visual Inputs / Outputs
        foreach(InputCell input in gridManager.GetInputs())
        {
            if(!inputs.ContainsKey(input))
            {
                //Generate Prefab at position
                GameObject prefab = Instantiate(inputCellPrefab, gridParent);
                prefab.GetComponent<RectTransform>().anchoredPosition = visualGridManager.GetScreenFromGrid(input.Coordinates[0]);
                prefab.GetComponentInChildren<Text>().text = input.InputValue.ToString();

                inputs.Add(input, prefab);
            }
        }

        foreach (OutputCell output in gridManager.GetOutputs())
        {
            if (!outputs.ContainsKey(output))
            {
                //Generate Prefab at position
                GameObject prefab = Instantiate(outputCellPrefab, gridParent);
                prefab.GetComponent<RectTransform>().anchoredPosition = visualGridManager.GetScreenFromGrid(output.Coordinates[0]);
                prefab.GetComponentInChildren<Text>().text = output.OutputTarget.ToString();

                outputs.Add(output, prefab);
            }
        }
    }

    private bool IsWaveBeaten()
    {
        return gridManager.IsSolved;
    }

    private int GetWaveClearScore()
    {
        return 0;
    }

    private float GetWaveClearTime()
    {
        return 0.0f;
    }

    private void Update()
    {
        //Update UI Elements
        if (isPlaying)
        {
            if (timerText != null)
            {
                timerText.text = (timeLeft > 0 ? Mathf.Floor(timeLeft) + 1 : 0).ToString();
            }

            if (wave >= 0 && waveText != null)
            {
                waveText.text = "Wave " + (wave + 1);
            }

            if(scoreText != null)
            {
                scoreText.text = score.ToString();
            }
        }
    }

    public void AddGate(GateType _type, CellCoordinates _coordinates, ObjectOrientation _orientation, GameObject _selfGameObject)
    {
        //Check if an object is already there
        GridObject gridObject = gridManager.GetCell(_coordinates);
        if(gridObject != null)
        {
            if (gridObject.ObjectType != GridObjectType.Wire)
            {
                Destroy(placedGridObjects[gridObject]);
                placedGridObjects.Remove(gridObject);           
            }
            else
            {
                wireVisualManager.ClearWire(_coordinates);
            }

            gridManager.ClearCell(_coordinates);
            gridObject = null;
        }

       switch(_type)
        {
            case GateType.Add:
            {
                gridObject = new AddGate(_coordinates, _orientation);
                break;
            }
            case GateType.Subtract:
            {
                gridObject = new SubtractGate(_coordinates, _orientation);
                break;
            }
            case GateType.Multiply:
            {
                gridObject = new MultiplyGate(_coordinates, _orientation);
                break;
            }
            case GateType.Divide:
            {
                gridObject = new DivideGate(_coordinates, _orientation);
                break;
            }
            case GateType.IncrementDecrement:
            {
                gridObject = new IncrementDecrementGate(_coordinates, _orientation);
                break;
            }
            case GateType.Cross:
            {
                gridObject = new CrossGate(_coordinates, _orientation);
                break;
            }
        }

        gridManager.InsertObject(gridObject);
        placedGridObjects.Add(gridObject, _selfGameObject);

        //Create Visual Wire
        Gate gate = (Gate)gridObject;
        foreach (CellCoordinates inputCoords in gate.Inputs)
        {
            GridObject inputGridObject = gridManager.GetCell(inputCoords);
            if (inputGridObject != null && inputGridObject.ObjectType != GridObjectType.Wire)
            {
                wireVisualManager.CreateWireAndLink(inputCoords, _coordinates, false);
            }
        }
        foreach (CellCoordinates outputCoords in gate.Outputs)
        {
            GridObject outputGridObject = gridManager.GetCell(outputCoords);
            if (outputGridObject != null && outputGridObject.ObjectType != GridObjectType.Wire)
            {
                wireVisualManager.CreateWireAndLink(_coordinates, outputCoords, true);
            }
        }
    }

    public void AddIncrementDecrementGate(CellCoordinates _coordinates, ObjectOrientation _orientation, int _value, GameObject _selfGameObject)
    {
        //Check if an object is already there
        GridObject gridObject = gridManager.GetCell(_coordinates);
        if (gridObject != null)
        {
            gridManager.ClearCell(_coordinates);

            Destroy(placedGridObjects[gridObject]);
            placedGridObjects.Remove(gridObject);
            gridObject = null;
        }

        gridObject = new IncrementDecrementGate(_coordinates, _orientation, _value);
        gridManager.InsertObject(gridObject);
        placedGridObjects.Add(gridObject, _selfGameObject);

        //Create Visual Wire
        Gate gate = (Gate)gridObject;
        foreach (CellCoordinates inputCoords in gate.Inputs)
        {
            GridObject inputGridObject = gridManager.GetCell(inputCoords);
            if (inputGridObject != null && inputGridObject.ObjectType != GridObjectType.Wire)
            {
                wireVisualManager.CreateWireAndLink(inputCoords, _coordinates, false);
            }
        }
        foreach (CellCoordinates outputCoords in gate.Outputs)
        {
            GridObject outputGridObject = gridManager.GetCell(outputCoords);
            if (outputGridObject != null && outputGridObject.ObjectType != GridObjectType.Wire)
            {
                wireVisualManager.CreateWireAndLink(_coordinates, outputCoords, true);
            }
        }
    }

    public void ClearCell(CellCoordinates _coordinates)
    {
        GridObject gridObject = gridManager.GetCell(_coordinates);
        if (gridObject != null && gridObject.ObjectType == GridObjectType.Gate)
        {
            Gate gate = (Gate)gridObject;
            foreach (CellCoordinates inputCoords in gate.Inputs)
            {
                wireVisualManager.ClearWire(inputCoords);
            }
            foreach (CellCoordinates outputCoords in gate.Outputs)
            {
                wireVisualManager.ClearWire(outputCoords);
            }
        }

        gridManager.ClearCell(_coordinates);
    }

    struct GameInputOutput
    {
        Vector2Int m_position;
        int m_value;
    }

	/// <summary>
	///  Ignore this.
	/// </summary>
	private void SetupStartingLevels()
	{
		m_levels = new Dictionary<string, LevelFile>();
		List<InputCell> inputs = new List<InputCell>();
		List<OutputCell> outputs = new List<OutputCell>();
		int numStartingOutputs = 1;

		// Level One
		inputs = new List<InputCell>();
		outputs = new List<OutputCell>();
		numStartingOutputs = 1;

		inputs.Add( new InputCell( new CellCoordinates( 0, 1 ), ObjectOrientation.Or0, 2 ) );
		inputs.Add( new InputCell( new CellCoordinates( 0, 2 ), ObjectOrientation.Or0, 3 ) );
		inputs.Add( new InputCell( new CellCoordinates( 0, 3 ), ObjectOrientation.Or0, 5 ) );
		inputs.Add( new InputCell( new CellCoordinates( 0, 4 ), ObjectOrientation.Or0, 1 ) );

		outputs.Add( new OutputCell( new CellCoordinates( 6, 2 ), ObjectOrientation.Or0, 3 ) );
		outputs.Add( new OutputCell( new CellCoordinates( 6, 4 ), ObjectOrientation.Or0, 6 ) );
		outputs.Add( new OutputCell( new CellCoordinates( 6, 5 ), ObjectOrientation.Or0, 2 ) );

		m_levels.Add( "one", new LevelFile( inputs, outputs, numStartingOutputs ) );
}
}
