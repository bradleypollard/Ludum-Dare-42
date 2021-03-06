﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoboTools.Helpers;
using UnityEngine.UI;
using System.Linq;

public class GameplayManager : MonoBehaviour
{
    //Debug Variables
    public bool debug_StartGameOnLoad = false;
	public string debug_LevelToLoad = "";

    //Game Constants
    public float StartTimeLeft = 100;
	public float WaveClearTime = 30.0f;

    //Game Variables
    private float timeLeft;
    private int wave;
    private bool isPlaying;

    private Dictionary<InputCell, GameObject> inputs;
    private Dictionary<OutputCell, GameObject> outputs;
    private Dictionary<GridObject, GameObject> placedGridObjects;

    //UI Accessors (Cause Lazy)
    public Text timerText;
    public Text waveText;
	public Text TimeUpText;
	public Text LevelCompleteText;
	public Text ScoreText;

    public Image background, fadeLayer;
    public GameObject mainMenu, gameInterface;

    public GameObject gridModeIndicator;
    private bool isGridModeIndicatorAnimating = false, isGridModeIndicatorHidden = false;

    public RectTransform gridParent, scrollViewParent;
    public GameObject inputCellPrefab, outputCellPrefab;

    public RectTransform timerImage;

    //Components
    private GridManager gridManager;
    private WireVisualManager wireVisualManager;
    private WireManager wireManager;
    private VisualGridManager visualGridManager;
    private LevelSelectButtonsGenerator levelSelectButtonsGenerator;
    private LevelButtonGenerator levelButtonGenerator;

    private readonly Color solvedColour = new Color(0.423f, 0.858f, 0.612f);
    private readonly Color unsolvedColour = new Color(0.925f, 0.941f, 0.945f);
    private readonly Color unsolvedWireColour = new Color(0.745f, 0.098f, 0.0f);

    private Dictionary<string, LevelFile> m_levels;
    public Color backgroundColour;

    private Color actualBackgroundColour;
    private Color orginalBackgroundColour;

    private bool isBackgroundFading = false;

    // Use this for initialization
    void Start ()
    {
		SetupStartingLevels();

        //Get Components
        gridManager = FindObjectOfType<GridManager>();
        wireVisualManager = FindObjectOfType<WireVisualManager>();
        wireManager = FindObjectOfType<WireManager>();
		visualGridManager = FindObjectOfType<VisualGridManager>();
        levelSelectButtonsGenerator = FindObjectOfType<LevelSelectButtonsGenerator>();
        levelButtonGenerator = FindObjectOfType<LevelButtonGenerator>();

        //Do Debug Logic
        if (debug_StartGameOnLoad)
        {
            StartGame("");
        }

        //Hide GridModeIndicator
        Color startColour = gridModeIndicator.GetComponent<Text>().color;
        startColour.a = 0.0f;
        gridModeIndicator.GetComponent<Text>().color = startColour;

        actualBackgroundColour = backgroundColour;
        orginalBackgroundColour = backgroundColour;
    }

	public void QuitGame()
	{
		StartCoroutine( OnQuit() );
	}

	private IEnumerator OnQuit()
	{
		// Transition

		Color clearedBackground = backgroundColour;
		clearedBackground.a = 0.0f;
		yield return FadeBackground( clearedBackground, Color.black, fadeLayer, 1.0f );
		
		Application.Quit();
	}

    public void StartGame(string levelName)
    {
        //Reset Values
        timeLeft = StartTimeLeft;
        wave = -1;
        isPlaying = true;
        inputs = new Dictionary<InputCell, GameObject>();
        outputs = new Dictionary<OutputCell, GameObject>();
        placedGridObjects = new Dictionary<GridObject, GameObject>();
		Color timeUpColour = TimeUpText.color;
		timeUpColour.a = 0;
		TimeUpText.color = timeUpColour;
		Color levelCompleteColour = LevelCompleteText.color;
		levelCompleteColour.a = 0.0f;
		LevelCompleteText.color =  levelCompleteColour;
		Color scoreTextColour = ScoreText.color;
		scoreTextColour.a = 0.0f;
		ScoreText.color = scoreTextColour;
		timerText.color = unsolvedColour;

		StartCoroutine(LoadLevel(levelName));
    }

    private IEnumerator LoadLevel(string _levelName)
    {
        //Deactive Menu Input

        //Transition
        Color clearedBackground = backgroundColour;
        clearedBackground.a = 0.0f;
        yield return FadeBackground(clearedBackground, backgroundColour, fadeLayer, 1.0f);
        fadeLayer.raycastTarget = true;

        gameInterface.SetActive(true);
        mainMenu.SetActive(false);

        //Load Inputs / Outputs
        LevelFile file = null;
		if ( debug_LevelToLoad != "" )
		{
			file = m_levels[debug_LevelToLoad];
		}
		else if ( _levelName != "" )
		{
			file = m_levels[_levelName];
		}
		yield return null; // yield here because in the future this will parse XML...

        //Setup Grid Manager
		gridManager.Initialise( file );
		yield return null;

        //Setup Visual Grid Manager
		visualGridManager.Initialise();
		wireVisualManager.Initialise();   

        //Transition
		if ( file != null )
		{
			orginalBackgroundColour = file.BGColour;
			backgroundColour = file.BGColour;
			levelButtonGenerator.RegenerateButtons( file.Buttons, file.IncDecValues );
		}
		else
		{
			orginalBackgroundColour = new Color( Random.Range( 0.0f, 0.5f ), Random.Range( 0.0f, 0.5f ), Random.Range( 0.0f, 0.5f ) );
			backgroundColour = orginalBackgroundColour;
			levelButtonGenerator.RegenerateButtons(
				new List<GateType> { GateType.Add, GateType.Subtract, GateType.IncrementDecrement, GateType.Multiply, GateType.Divide, GateType.Cross, GateType.Replicate },
				new List<int> { 2, 1, -1, -2 }
			);
		}

        UpdateCells();

        yield return FadeBackground(fadeLayer.color, clearedBackground, fadeLayer, 1.0f);
        fadeLayer.raycastTarget = false;
        yield return null;

        //Activate Player Input

        yield return StartCoroutine(GameLoop( file ) );

        //Stop Player from inputing
        fadeLayer.raycastTarget = true;

        yield return new WaitForSeconds(1f);
    
        levelSelectButtonsGenerator.RegenerateLevels();

        yield return new WaitForSeconds(1f);

        //Transition back
        clearedBackground = backgroundColour;
		clearedBackground.a = 0.0f;
		yield return FadeBackground( clearedBackground, backgroundColour, fadeLayer, 1.0f );

        //Clear visual grid
        foreach (GameObject input in inputs.Values)
        {
            Destroy(input);
        }
        foreach (GameObject output in outputs.Values)
        {
            Destroy(output);
        }
        foreach (GameObject grid in placedGridObjects.Values)
        {
            Destroy(grid);
        }
        foreach (WireVisualManager.VisualWire visualWire in wireVisualManager.GetCompletedWires())
        {
            foreach (GameObject wire in visualWire.wireObjects)
            {
                Destroy(wire);
            }
        }

        levelButtonGenerator.ClearButtons();

        gameInterface.SetActive( false );
		mainMenu.SetActive( true );

		yield return FadeBackground( fadeLayer.color, clearedBackground, fadeLayer, 1.0f );
        fadeLayer.raycastTarget = false;
	}

    private IEnumerator GameLoop( LevelFile _file )
    {
        while (isPlaying)
        {
			bool completedAllWaves = false;
            while(isPlaying && !IsWaveBeaten())
            {
                timeLeft -= Time.deltaTime;

                timerImage.Rotate(new Vector3(0, 0, -180.0f * Time.deltaTime));

                if (timeLeft <= 0.0f)
                {
                    isPlaying = false;

                    //Random Level
                    if (_file == null)
                    {
                        PlayerPrefs.SetInt("InfiniteScore", wave);
                    }
                }

                ColourCells(false, true);

                yield return null;
            }

            ColourCells(true, false);
			
			if ( isPlaying )
			{
				isPlaying = GenerateWave();
				if ( !isPlaying )
				{
					completedAllWaves = true;
				}
			}

            if( !isPlaying )
            {
                wireManager.EndMode();

                if ( completedAllWaves )
				{
					int score = gridManager.GetEmptyCells();
					ScoreText.text = score + " EMPTY CELLS";
					LevelCompleteText.gameObject.SetActive( true );
					if ( _file != null )
					{
						int highScore = PlayerPrefs.GetInt( _file.Name + "Score", -1 );
						if ( score > highScore )
						{
							PlayerPrefs.SetInt( _file.Name + "Score", score );
						}

                        //Set Max Levels
                        PlayerPrefs.SetInt(_file.Name + "Complete", 1);
                    }
					yield return FadeInText( LevelCompleteText, ScoreText, 0.5f );
				}
				else
				{
					timerText.color = unsolvedWireColour;
					timerText.text = "0";

                    yield return FadeInText( TimeUpText, 0.5f );
				}
				yield return new WaitForSeconds(1.0f);
            }
			else
			{
				timeLeft += GetWaveClearTime();
			}
        }

        ColourCells(true, false);
        Debug.Log("GAME OVER");
    }

    private bool GenerateWave()
    {
        bool canAdvance = true;

        //Increment Wave
        wave++;

		canAdvance = gridManager.AdvanceGeneration();
        UpdateCells();

        return canAdvance;
    }

    private void UpdateCells()
    {
        //Update Visual Inputs / Outputs
        foreach (InputCell input in gridManager.GetInputs())
        {
            if (!inputs.ContainsKey(input))
            {
                //Generate Prefab at position
                GameObject prefab = Instantiate(inputCellPrefab, gridParent);
                prefab.GetComponent<RectTransform>().anchoredPosition = visualGridManager.GetScreenFromGrid(input.Coordinates[0]);
                prefab.GetComponent<RectTransform>().localScale = Vector3.one * (5.0f / (Mathf.Min(visualGridManager.gridWidth, visualGridManager.gridHeight)));

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
                prefab.GetComponent<RectTransform>().localScale = Vector3.one * (5.0f / (Mathf.Min(visualGridManager.gridWidth, visualGridManager.gridHeight)));

                prefab.GetComponentInChildren<Text>().text = output.OutputTarget.ToString();

                outputs.Add(output, prefab);
            }
        }
    }

    private void ColourCells(bool _doWiresSolved = false, bool _doWiresUnSolved = false)
    {
        //Update Outputs
        foreach (KeyValuePair<OutputCell, GameObject> output in outputs)
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

        //Colour Wires
        if (_doWiresSolved)
        {
            wireVisualManager.ColourWires(solvedColour);
        }
        if (_doWiresUnSolved)
        {
            wireVisualManager.ColourWires(unsolvedWireColour);
        }
    }

    private bool IsWaveBeaten()
    {
        return gridManager.IsSolved;
    }

    private float GetWaveClearTime()
    {
        return WaveClearTime;
    }

    private void Update()
    {
        //Update UI Elements
        if (isPlaying)
        {
            if (timerText != null)
            {
                timerText.text = (timeLeft > 0 ? (timeLeft < 100 ?Mathf.Floor(timeLeft) + 1 : 100) : 0).ToString();
            }

            if (wave >= 0 && waveText != null)
            {
                waveText.text = "Wave " + (wave + 1);
            }
        }

        if(wireManager.IsInWireEditMode() != isGridModeIndicatorHidden && !isGridModeIndicatorAnimating)
        {
            isGridModeIndicatorAnimating = true;
            isGridModeIndicatorHidden = wireManager.IsInWireEditMode();

            StartCoroutine(FadeInText(gridModeIndicator.GetComponent<Text>(), wireManager.IsInWireEditMode()));

            if(isGridModeIndicatorHidden)
            {
                backgroundColour = Color.Lerp(orginalBackgroundColour, Color.black, 0.5f);
            }
            else
            {
                backgroundColour = orginalBackgroundColour;
            }
        }

        if(actualBackgroundColour != backgroundColour && !isBackgroundFading)
        {
            isBackgroundFading = true;
            StartCoroutine(FadeBackground(actualBackgroundColour, backgroundColour, background));
            StartCoroutine(TurnOffAfterFade(backgroundColour));
        }
    }

    private IEnumerator FadeInText(Text _text, bool _show)
    {
        float startTime = Time.time, fadeTime = 0.25f;
        Color startColour = _text.color;
        float startAlpha = startColour.a;

        while (Time.time - startTime < fadeTime)
        {
            startColour.a = Mathf.Lerp(startAlpha, _show ? 1.0f : 0.0f, (Time.time - startTime)/ fadeTime);
            _text.color = startColour;
            yield return null;
        }

        startColour.a = _show ? 1.0f : 0.0f;
        _text.color = startColour;

        isGridModeIndicatorAnimating = false;
    }

    private IEnumerator TurnOffAfterFade(Color _end)
    {
        yield return new WaitForSeconds(0.25f);

        isBackgroundFading = false;
        actualBackgroundColour = _end;
    }

    private IEnumerator FadeBackground(Color _start, Color _end, Image _image, float _fadeTime = 0.25f)
    {
        float startTime = Time.time;
        while (Time.time - startTime < _fadeTime)
        {
            _image.color = Color.Lerp(_start, _end, (Time.time - startTime) / _fadeTime);
            yield return null;
        }

        _image.color = _end;
    }

	private IEnumerator FadeInText( Text _text, float _fadeTime = 0.25f )
	{
		Color startColour = _text.color;
		startColour.a = 0.0f;
		Color endColour = _text.color;
		endColour.a = 1.0f;

		float startTime = Time.time;
		while ( Time.time - startTime < _fadeTime )
		{
			_text.color = Color.Lerp( startColour, endColour, ( Time.time - startTime ) / _fadeTime );
			yield return null;
		}

		_text.color = endColour;
	}

	private IEnumerator FadeInText( Text _text1, Text _text2, float _fadeTime = 0.25f )
	{
		Color startColour1 = _text1.color;
		startColour1.a = 0.0f;
		Color endColour1 = _text1.color;
		endColour1.a = 1.0f;

		Color startColour2 = _text2.color;
		startColour2.a = 0.0f;
		Color endColour2 = _text2.color;
		endColour2.a = 1.0f;

		float startTime = Time.time;
		while ( Time.time - startTime < _fadeTime )
		{
			_text1.color = Color.Lerp( startColour1, endColour1, ( Time.time - startTime ) / _fadeTime );
			_text2.color = Color.Lerp( startColour2, endColour2, ( Time.time - startTime ) / _fadeTime );
			yield return null;
		}

		_text1.color = endColour1;
		_text2.color = endColour2;
	}

	public void AddGate(GateType _type, CellCoordinates _coordinates, ObjectOrientation _orientation, GameObject _selfGameObject, int _value = 0, bool _isSwap = false)
    {
        //Check if an object is already there
        GridObject gridObject = gridManager.GetCell(_coordinates);
        if(gridObject != null)
        {
            ClearCell(_coordinates, _isSwap);
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
                gridObject = new IncrementDecrementGate(_coordinates, _orientation, _value);
                break;
            }
            case GateType.Cross:
            {
                gridObject = new CrossGate(_coordinates, _orientation);
                break;
            }
			case GateType.Replicate:
			{
				gridObject = new ReplicateGate( _coordinates, _orientation );
				break;
			}
		}

        gridManager.InsertObject(gridObject);
        placedGridObjects.Add(gridObject, _selfGameObject);

        //Create Visual Wire
        Gate gate = (Gate)gridObject;
        uint index = 0;
        foreach (CellCoordinates inputCoords in gate.Inputs)
        {
            GridObject inputGridObject = gridManager.GetCell(inputCoords);
            if (inputGridObject != null)
            {
                if (inputGridObject.ObjectType != GridObjectType.Wire)
                {
					if ( IsConnected( false, inputCoords , gridObject) )
					{
						wireVisualManager.CreateWireAndLink( inputCoords, gate.GetCoordinateForInput( index ) );
					}
                }
            }
            index++;
        }

        index = 0;
        foreach (CellCoordinates outputCoords in gate.Outputs)
        {
            GridObject outputGridObject = gridManager.GetCell(outputCoords);
            if (outputGridObject != null)
            {
                if (outputGridObject.ObjectType != GridObjectType.Wire)
                {
					if ( IsConnected( true, outputCoords , gridObject) )
					{
						wireVisualManager.CreateWireAndLink( _coordinates, outputCoords );
					}
                }
            }
            index++;
        }

        UpdateGiblets(_coordinates, true);
    }

    public void ClearCell(CellCoordinates _coordinates, bool _isSwap)
    {
        GridObject gridObject = gridManager.GetCell(_coordinates);
        List<CellCoordinates> neighboursToInformOfDeath = new List<CellCoordinates>();

        //Clear Wires
        if (gridObject != null && gridObject.ObjectType == GridObjectType.Gate)
        {
            foreach (CellCoordinates cell in gridObject.Coordinates)
            {
                Gate gate = (Gate)gridManager.GetCell(cell);

                foreach (CellCoordinates inputCoords in gate.Inputs)
                {
                    GridObject input = gridManager.GetCell(inputCoords);
                    if (input != null)
                    {
                        if (input.ObjectType == GridObjectType.Wire)
                        {
                            Wire wire = (Wire)input;
                            for (uint i = 0; i < gridObject.Coordinates.Length; ++i)
                            {
                                if (gridObject.Coordinates[i] == wire.Exit)
                                {
                                    // This wire actually connects to us, destroy it
                                    wireVisualManager.ClearWire(inputCoords);
                                    break;
                                }
                            }
                        }

                        neighboursToInformOfDeath.Add(inputCoords);
                    }
                }
                foreach (CellCoordinates outputCoords in gate.Outputs)
                {
                    GridObject output = gridManager.GetCell(outputCoords);
                    if (output != null)
                    {
                        if (output.ObjectType == GridObjectType.Wire)
                        {
                            Wire wire = (Wire)output;
                            for (uint i = 0; i < gridObject.Coordinates.Length; ++i)
                            {
                                if (gridObject.Coordinates[i] == wire.Entry)
                                {
                                    // This wire actually connects to us, destroy it
                                    wireVisualManager.ClearWire(outputCoords);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            wireVisualManager.ClearWire(outputCoords);
                        }

                        neighboursToInformOfDeath.Add(outputCoords);
                    }
                }
            }
        }

        if (gridObject != null)
        {
            if (gridObject.ObjectType != GridObjectType.Wire)
            {
                if (!_isSwap)
                {
                    Destroy(placedGridObjects[gridObject]);
                    placedGridObjects.Remove(gridObject);
                }
            }

            wireVisualManager.ClearWire(_coordinates);
        }

        gridManager.ClearCell(_coordinates);

        foreach(CellCoordinates cell in neighboursToInformOfDeath)
        {
            UpdateGiblets(cell, false);
        }
    }

    public void UpdateGiblets(CellCoordinates _coordinates, bool _updateNeighbour = false)
    {
        GridObject gridObject = gridManager.GetCell(_coordinates);
        if (gridObject != null && gridObject.ObjectType == GridObjectType.Gate)
        {
            foreach (CellCoordinates coords in gridObject.Coordinates)
            {
                Gate gate = (Gate)gridManager.GetCell(coords);

                int count = 1;
                foreach (CellCoordinates inputCoords in gate.Inputs)
                {
                    if (IsConnected(false, inputCoords, gridObject))
                    {
                        //Hide Giblets
                        placedGridObjects[gridObject].transform.Find("InConnector_" + count).gameObject.SetActive(false);

                        if (_updateNeighbour)
                        {
                            // Neighbour is gate so update their giblets too
                            UpdateGiblets(inputCoords);
                        }
                    }
                    else
                    {
                        // Show giblets
                        placedGridObjects[gridObject].transform.Find("InConnector_" + count).gameObject.SetActive(true);
                    }

                    count++;
                }

                count = 1;
                foreach (CellCoordinates outputCoords in gate.Outputs)
                {
                    if (IsConnected(true, outputCoords, gridObject))
                    {
                        //Hide Giblets
                        placedGridObjects[gridObject].transform.Find("OutConnector_" + count).gameObject.SetActive(false);

                        if (_updateNeighbour)
                        {
                            // Neighbour is gate so update their giblets too
                            UpdateGiblets(outputCoords);
                        }
                    }
                    else
                    {
                        placedGridObjects[gridObject].transform.Find("OutConnector_" + count).gameObject.SetActive(true);
                    }

                    count++;
                }
            }

            //Create Visual Wire
            {
                Gate gate = (Gate)gridObject;
                uint index = 0;
                foreach (CellCoordinates inputCoords in gate.Inputs)
                {
                    GridObject inputGridObject = gridManager.GetCell(inputCoords);
                    if (inputGridObject != null)
                    {
                        if (inputGridObject.ObjectType != GridObjectType.Wire)
                        {
                            if (IsConnected(false, inputCoords, gridObject))
                            {
                                wireVisualManager.CreateWireAndLink(inputCoords, gate.GetCoordinateForInput(index));
                            }
                        }
                    }
                    index++;
                }

                index = 0;
                foreach (CellCoordinates outputCoords in gate.Outputs)
                {
                    GridObject outputGridObject = gridManager.GetCell(outputCoords);
                    if (outputGridObject != null)
                    {
                        if (outputGridObject.ObjectType != GridObjectType.Wire)
                        {
                            if (IsConnected(true, outputCoords, gridObject))
                            {
                                wireVisualManager.CreateWireAndLink(_coordinates, outputCoords);
                            }
                        }
                    }
                    index++;
                }
            }
        }
    }
	
    private bool IsConnected(bool _isOutput, CellCoordinates _coords, GridObject _startObject)
    {
        //Check Us
		GridObject gridObject = gridManager.GetCell( _coords );

        if ( gridObject != null )
		{
			switch ( gridObject.ObjectType )
			{
                case GridObjectType.Wire:
                {
                    Wire wire = (Wire)gridObject;
                    for (uint i = 0; i < _startObject.Coordinates.Length; ++i)
                    {
                        CellCoordinates toCompare = _isOutput ? wire.Entry : wire.Exit;
                        if (_startObject.Coordinates[i] == toCompare)
                        {
                            return true;
                        }
                    }
                    break;
                }
                case GridObjectType.Input:
                {
                    InputCell cell = (InputCell)gridObject;
                    for (uint i = 0; i < _startObject.Coordinates.Length; ++i)
                    {
                        if (_startObject.Coordinates[i] == cell.Exit)
                        {
                            if (_startObject.ObjectType == GridObjectType.Gate)
                            {
                                Gate usAsGate = (Gate)gridManager.GetCell(_startObject.Coordinates[i]);

                                foreach (CellCoordinates input in usAsGate.Inputs)
                                {
                                    if (input == _coords)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {

                                return true;
                            }
                        }
                    }
                    break;
                }
                case GridObjectType.Output:
				{
					OutputCell cell = (OutputCell)gridObject;
                    for (uint i = 0; i < _startObject.Coordinates.Length; ++i)
                    {
						if (_startObject.Coordinates[i] == cell.Entry )
						{
                            if (_startObject.ObjectType == GridObjectType.Gate)
                            {
                                Gate usAsGate = (Gate)gridManager.GetCell(_startObject.Coordinates[i]);

                                foreach (CellCoordinates output in usAsGate.Outputs)
                                {
                                    if (output == _coords)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {

                                return true;
                            }
                        }
					}
					break;
				}
				case GridObjectType.Gate:
				{
                    Gate gateCell = (Gate)gridObject;
                    for (uint i = 0; i < _startObject.Coordinates.Length; ++i)
                    {
                        for (uint j = 0; j < (_isOutput? gateCell.Inputs.Length : gateCell.Outputs.Length); ++j)
                        {
                            CellCoordinates[] toCompare = _isOutput ? gateCell.Inputs : gateCell.Outputs;
                            if (_startObject.Coordinates[i] == toCompare[j])
                            {
                                return true;
                            }
                        }
                    }

                    break;
                }
			}
		}
		return false;
	}

	struct GameInputOutput
    {
        Vector2Int m_position;
        int m_value;
    }

	// This will be XML one day, honest.
	private void SetupStartingLevels()
	{
		m_levels = new Dictionary<string, LevelFile>();

		// Values
		string name = "";
		List<InputCell> inputs = new List<InputCell>();
		List<OutputCell> outputs = new List<OutputCell>();
		List<GateType> buttons = new List<GateType>{ GateType.Add, GateType.Subtract, GateType.IncrementDecrement, GateType.Multiply, GateType.Divide, GateType.Cross, GateType.Replicate };
		List<int> incrementDecrementValues = new List<int>(); 
		int numStartingOutputs = 1;
		int dimensionX = 5;
		int dimensionY = 5;
		Color color;

		// Level One
		name = "one";
		inputs = new List<InputCell>();
		outputs = new List<OutputCell>();
		buttons = new List<GateType> { GateType.Add, GateType.Subtract, GateType.IncrementDecrement };
		incrementDecrementValues = new List<int> { 1, -1 };
		numStartingOutputs = 1;
		dimensionX = 3;
		dimensionY = 3;
		ColorUtility.TryParseHtmlString( "#439C89", out color );

		inputs.Add( new InputCell( new CellCoordinates( 0, 3 ), ObjectOrientation.Or0, 1 ) );
		inputs.Add( new InputCell( new CellCoordinates( 0, 1 ), ObjectOrientation.Or0, 1 ) );

		outputs.Add( new OutputCell( new CellCoordinates( 4, 3 ), ObjectOrientation.Or0, 1 ) );
		outputs.Add( new OutputCell( new CellCoordinates( 4, 1 ), ObjectOrientation.Or0, 2 ) );

		m_levels.Add( name, new LevelFile( name, inputs, outputs, buttons, incrementDecrementValues, numStartingOutputs, dimensionX, dimensionY, color ) );


		// Level Two
		name = "two";
		inputs = new List<InputCell>();
		outputs = new List<OutputCell>();
		buttons = new List<GateType> { GateType.Add, GateType.Subtract, GateType.IncrementDecrement };
		incrementDecrementValues = new List<int> { 1, -1 };
		numStartingOutputs = 1;
		dimensionX = 3;
		dimensionY = 3;
		ColorUtility.TryParseHtmlString( "#9D8844", out color );

		inputs.Add( new InputCell( new CellCoordinates( 0, 1 ), ObjectOrientation.Or0, 3 ) );
		inputs.Add( new InputCell( new CellCoordinates( 0, 2 ), ObjectOrientation.Or0, 7 ) );
		inputs.Add( new InputCell( new CellCoordinates( 0, 3 ), ObjectOrientation.Or0, 2 ) );

		outputs.Add( new OutputCell( new CellCoordinates( 4, 2 ), ObjectOrientation.Or0, 10 ) );
		outputs.Add( new OutputCell( new CellCoordinates( 4, 3 ), ObjectOrientation.Or0, 3 ) );

		m_levels.Add( name, new LevelFile( name, inputs, outputs, buttons, incrementDecrementValues, numStartingOutputs, dimensionX, dimensionY, color ) );


		// Level Three
		name = "three";
		inputs = new List<InputCell>();
		outputs = new List<OutputCell>();
		buttons = new List<GateType> { GateType.IncrementDecrement, GateType.Add, GateType.Subtract, GateType.Replicate };
		incrementDecrementValues = new List<int> { 1, -1 };
		numStartingOutputs = 1;
		dimensionX = 3;
		dimensionY = 3;
		ColorUtility.TryParseHtmlString( "#9D8844", out color );

		inputs.Add( new InputCell( new CellCoordinates( 2, 4 ), ObjectOrientation.Or90, 3 ) );
		inputs.Add( new InputCell( new CellCoordinates( 0, 2 ), ObjectOrientation.Or0, 2 ) );
		inputs.Add( new InputCell( new CellCoordinates( 0, 1 ), ObjectOrientation.Or0, 1 ) );

		outputs.Add( new OutputCell( new CellCoordinates( 4, 1 ), ObjectOrientation.Or0, 2 ) );
		outputs.Add( new OutputCell( new CellCoordinates( 4, 2 ), ObjectOrientation.Or0, 5 ) );
		outputs.Add( new OutputCell( new CellCoordinates( 4, 3 ), ObjectOrientation.Or0, 3 ) );

		m_levels.Add( name, new LevelFile( name, inputs, outputs, buttons, incrementDecrementValues, numStartingOutputs, dimensionX, dimensionY, color ) );


		// Level Four
		name = "four";
		inputs = new List<InputCell>();
		outputs = new List<OutputCell>();
		buttons = new List<GateType> { GateType.IncrementDecrement, GateType.Add, GateType.Subtract, GateType.Cross, GateType.Replicate };
		incrementDecrementValues = new List<int> { 1, -1 };
		numStartingOutputs = 1;
		dimensionX = 5;
		dimensionY = 5;
		ColorUtility.TryParseHtmlString( "#571007", out color );

		inputs.Add( new InputCell( new CellCoordinates( 0, 1 ), ObjectOrientation.Or0, 2 ) );
		inputs.Add( new InputCell( new CellCoordinates( 0, 2 ), ObjectOrientation.Or0, 3 ) );
		inputs.Add( new InputCell( new CellCoordinates( 0, 3 ), ObjectOrientation.Or0, 5 ) );
		inputs.Add( new InputCell( new CellCoordinates( 0, 4 ), ObjectOrientation.Or0, 1 ) );

		outputs.Add( new OutputCell( new CellCoordinates( 6, 2 ), ObjectOrientation.Or0, 3 ) );
		outputs.Add( new OutputCell( new CellCoordinates( 6, 4 ), ObjectOrientation.Or0, 6 ) );
		outputs.Add( new OutputCell( new CellCoordinates( 6, 5 ), ObjectOrientation.Or0, 2 ) );

		m_levels.Add( name, new LevelFile( name, inputs, outputs, buttons, incrementDecrementValues, numStartingOutputs, dimensionX, dimensionY, color ) );


		// Level Five
		name = "five";
		inputs = new List<InputCell>();
		outputs = new List<OutputCell>();
		buttons = new List<GateType> { GateType.IncrementDecrement, GateType.Add, GateType.Subtract, GateType.Multiply, GateType.Divide, GateType.Cross, GateType.Replicate };
		incrementDecrementValues = new List<int> { 1, -1 };
		numStartingOutputs = 2;
		dimensionX = 7;
		dimensionY = 7;
		ColorUtility.TryParseHtmlString( "#571007", out color );

		inputs.Add( new InputCell( new CellCoordinates( 0, 4 ), ObjectOrientation.Or0, 8 ) );
		inputs.Add( new InputCell( new CellCoordinates( 0, 6 ), ObjectOrientation.Or0, 9 ) );
		inputs.Add( new InputCell( new CellCoordinates( 1, 0 ), ObjectOrientation.Or270, 2 ) );
		inputs.Add( new InputCell( new CellCoordinates( 3, 0 ), ObjectOrientation.Or270, 7 ) );
		inputs.Add( new InputCell( new CellCoordinates( 5, 0 ), ObjectOrientation.Or270, 3 ) );
		inputs.Add( new InputCell( new CellCoordinates( 4, 8 ), ObjectOrientation.Or90, 1 ) );
		inputs.Add( new InputCell( new CellCoordinates( 7, 8 ), ObjectOrientation.Or90, 5 ) );

		outputs.Add( new OutputCell( new CellCoordinates( 8, 4 ), ObjectOrientation.Or0, 16 ) );
		outputs.Add( new OutputCell( new CellCoordinates( 5, 8 ), ObjectOrientation.Or270, 11 ) );
		outputs.Add( new OutputCell( new CellCoordinates( 3, 8 ), ObjectOrientation.Or270, 4 ) );
		outputs.Add( new OutputCell( new CellCoordinates( 6, 8 ), ObjectOrientation.Or270, 9 ) );
		outputs.Add( new OutputCell( new CellCoordinates( 8, 1 ), ObjectOrientation.Or0, 35 ) );

		m_levels.Add( name, new LevelFile( name, inputs, outputs, buttons, incrementDecrementValues, numStartingOutputs, dimensionX, dimensionY, color ) );
	}

    public void ResetSave()
    {
        PlayerPrefs.DeleteAll();
        levelSelectButtonsGenerator.RegenerateLevels();
    }

    public void ReturnToTime()
    {
        timeLeft = 0.0f;
    }
}
