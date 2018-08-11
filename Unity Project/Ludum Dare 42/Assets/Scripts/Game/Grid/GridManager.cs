using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
	public uint DimensionX, DimensionY;
	public uint NumInputs = 3;
	public uint NumStartingOutputs = 1;
	public uint NumOutputsPerGeneration = 1;
	public int MaxInputValue = 10;
	public int MaxOutputTarget = 10;
	public bool IsSolved = false;

	private GridObject[,] m_grid;
	private uint m_generation;
	private List<InputCell> m_inputs;
	private List<OutputCell> m_outputs;

	// Use this for initialization
	void Start()
	{
		Initialise();
	}

	// Update is called once per frame
	void Update()
	{
		Solve();
	}

	///////////////////////// API /////////////////////////
	public void Initialise()
	{
		ClearGrid();
		GenerateInputs();
		GenerateOutputs();
	}

	public List<InputCell> GetInputs()
	{
		return m_inputs;
	}

	public List<OutputCell> GetOutputs()
	{
		return m_outputs;
	}

	public void AdvanceGeneration()
	{
		m_generation++;
		Debug.Log( "GridManager: Generation advanced - " + m_generation );
		GenerateOutputs();
	}

	public void InsertObject( GridObject _object )
	{
		// Inserts the given GridObject into all the coordinates it occupies
		for ( uint i = 0; i < _object.Coordinates.Length; ++i )
		{
			if ( m_grid[_object.Coordinates[i].X, _object.Coordinates[i].Y] != null )
			{
				Debug.LogError( "GridManager: Inserting a GridObject into an occupied cell - (" + _object.Coordinates[i].X.ToString() + "," + _object.Coordinates[i].Y.ToString() + ")" );
			}

			Debug.Log( "GridManager: Inserting a GridObject into (" + _object.Coordinates[i].X.ToString() + "," + _object.Coordinates[i].Y.ToString() + ")" );
			m_grid[_object.Coordinates[i].X, _object.Coordinates[i].Y] = _object;
		}
	}

	public void ClearCell( CellCoordinates _coordinates )
	{
		// Removes all references to the GridObject at _coordinates from the grid
		GridObject o = m_grid[_coordinates.X, _coordinates.Y];
		if ( o != null )
		{
			for ( uint i = 0; i < o.Coordinates.Length; ++i )
			{
				m_grid[o.Coordinates[i].X, o.Coordinates[i].Y] = null;
			}
		}
	}

	///////////////////////// Helpers /////////////////////////
	private void ClearGrid()
	{
		// Generate grid 2 cells larger than required for inputs and outputs.
		// Player can only edit the centre DimensionX x DimensionY grid.
		m_grid = new GridObject[DimensionX + 2, DimensionY + 2];
		m_generation = 0;
		m_inputs = new List<InputCell>();
		m_outputs = new List<OutputCell>();
	}

	private void GenerateInputs()
	{
		// Create the initial starting input GridObjects and place them in the grid.
		for ( uint i = 0; i < NumInputs; ++i )
		{
			GenerateInput();
		}
	}

	private void GenerateInput()
	{
		uint x = 0;
		uint y = (uint)Random.Range( 1, DimensionY );
		int value = Random.Range( 1, MaxInputValue );

		m_inputs.Add( new InputCell( new CellCoordinates( x, y ), ObjectOrientation.Or0, value ) );
		Debug.Log( "GridManager: Input added - (" + x + "," + y + ") value " + value );
	}

	private void GenerateOutputs()
	{
		// Generate outputs needed for the current generation, preserving existing outputs
		// for the generation.
		if ( m_generation == 0 )
		{
			for ( uint i = 0; i < NumStartingOutputs; ++i )
			{
				GenerateOutput();
			}
		}
		else
		{
			GenerateOutput();
		}
	}

	private void GenerateOutput()
	{
		uint x = DimensionX;
		uint y = (uint)Random.Range( 1, DimensionY );
		int target = Random.Range( 1, MaxOutputTarget );

		m_outputs.Add( new OutputCell( new CellCoordinates( x, y ), ObjectOrientation.Or0, target ) );
		Debug.Log( "GridManager: Output added - (" + x + "," + y + ") target " + target );
	}

	private void Solve()
	{
		// Attempt to solve the grid to determine if you are a good boi.
		IsSolved = true;


	}
}
