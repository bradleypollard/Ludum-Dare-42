using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoboTools.Helpers;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    //Debug Variables
    public bool debug_StartGameOnLoad = false;

    //Game Constants
    const float startTimeLeft = 100;

    //Game Variables
    private float timeLeft;
    private int score, wave, gridWidth, gridHeight;
    private bool isPlaying;

    private Dictionary<InputCell, GameObject> inputs;
    private Dictionary<OutputCell, GameObject> outputs;

    //UI Accessors (Cause Lazy)
    public Text timerText;
    public Text waveText;
    public Text scoreText;

    public RectTransform gridParent, scrollViewParent;
    public GameObject inputCellPrefab, outputCellPrefab;

    //Components
    private GridManager gridManager;

    private readonly Color solvedColour = new Color(0.423f, 0.858f, 0.612f);
    private readonly Color unsolvedColour = new Color(0.925f, 0.941f, 0.945f);

    // Use this for initialization
    void Start ()
    {
        //Get Components
        gridManager = FindObjectOfType<GridManager>();

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
            gridManager.Initialise();
        }
        else
        {
            gridManager.AdvanceGeneration();
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

    public void AddGate(GateType _type, CellCoordinates _coordinates, ObjectOrientation _orientation)
    {
       GridObject gridObject = null;
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
    }

    public void AddIncrementDecrementGate(CellCoordinates _coordinates, ObjectOrientation _orientation, int _value)
    {
        GridObject gridObject = new IncrementDecrementGate(_coordinates, _orientation, _value);
        gridManager.InsertObject(gridObject);
    }

    public void ClearCell(CellCoordinates _coordinates)
    {
        gridManager.ClearCell(_coordinates);
    }

    struct GameInputOutput
    {
        Vector2Int m_position;
        int m_value;
    }
}
