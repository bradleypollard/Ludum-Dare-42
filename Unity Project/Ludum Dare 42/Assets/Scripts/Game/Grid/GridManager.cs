using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
	public uint DimensionX, DimensionY;
	public uint NumInputs = 3;
	public uint NumStartingOutputs = 1;
	public uint NumOutputsPerGeneration = 1;
	public uint NumTotalOutputs = 5;
	public int MaxInputValue = 10;
	public int MaxOutputTarget = 10;
	public bool IsSolved = false;

	private GridObject[,] m_grid;
	private uint m_generation;
	private List<InputCell> m_inputs;
	private List<OutputCell> m_outputs;
	private List<OutputCell> m_futureOutputs;

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		if ( m_outputs != null && m_outputs.Count > 0 )
		{
			Solve();
		}
	}

	///////////////////////// API /////////////////////////
	public void Initialise( LevelFile _file = null )
	{
		ClearGrid();
		IsSolved = false;
		if ( _file != null )
		{
			LoadInputs( _file.Inputs );
			LoadOutputs( _file.Outputs, _file.NumStartingOutputs );
		}
		else
		{
			GenerateInputs();
			GenerateOutputs();
		}
	}

	public List<InputCell> GetInputs()
	{
		return m_inputs;
	}

	public List<OutputCell> GetOutputs()
	{
		return m_outputs;
	}

	public bool AdvanceGeneration()
	{
		m_generation++;
		Debug.Log( "GridManager: Generation advanced - " + m_generation );
		IsSolved = false;
		return AddNextOutput();
	}

	public void InsertObject( GridObject _object )
	{
		// Inserts the given GridObject into all the coordinates it occupies
		for ( uint i = 0; i < _object.Coordinates.Length; ++i )
		{
			if ( GetCell( _object.Coordinates[i] ) != null )
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
			// Delete wires (if any)
			if ( o.ObjectType == GridObjectType.Gate )
			{
				Gate gate = (Gate)o;
				foreach ( CellCoordinates inputCoords in gate.Inputs )
				{
					GridObject input = GetCell( inputCoords );
					if ( input != null && input.ObjectType == GridObjectType.Wire )
					{
						ClearCell( input.Coordinates[0] );
					}
				}
				foreach ( CellCoordinates outputCoords in gate.Outputs )
				{
					GridObject output = GetCell( outputCoords );
					if ( output != null && output.ObjectType == GridObjectType.Wire )
					{
						ClearCell( output.Coordinates[0] );
					}
				}
			}

			// Delete self
			for ( uint i = 0; i < o.Coordinates.Length; ++i )
			{
				Debug.Log( "GridManager: Clearing a GridObject from (" + o.Coordinates[i].X.ToString() + "," + o.Coordinates[i].Y.ToString() + ")" );
				m_grid[o.Coordinates[i].X, o.Coordinates[i].Y] = null;
			}
		}
	}

	public GridObject GetCell( CellCoordinates _coordinates )
	{
		return m_grid[_coordinates.X, _coordinates.Y];
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
		int value = Random.Range( 1, MaxInputValue + 1 );
		while ( GetCell( new CellCoordinates( x, y ) ) != null )
		{
			y = (uint)Random.Range( 1, DimensionY );
		}

		InputCell input = new InputCell( new CellCoordinates( x, y ), ObjectOrientation.Or0, value );
		m_inputs.Add( input );
		InsertObject( input );
		Debug.Log( "GridManager: Input added - (" + x + "," + y + ") value " + value );
	}

	private void LoadInputs( List<InputCell> _inputs )
	{
		foreach ( InputCell input in _inputs )
		{
			InsertObject( input );
			Debug.Log( "GridManager: Input added - " + input.Coordinates[0].ToString() + " value " + input.InputValue );
		}
		m_inputs = _inputs;
	}

	private void GenerateOutputs()
	{
		// Generate the initial outputs and populate the future outputs list
		for ( uint i = 0; i < NumTotalOutputs; ++i )
		{
			GenerateOutput( i < NumStartingOutputs );
		}
		IsSolved = false;
	}

	private void GenerateOutput( bool _insert )
	{
		uint x = DimensionX + 1;
		uint y = (uint)Random.Range( 1, DimensionY );
		int target = Random.Range( 1, MaxOutputTarget + 1 );
		while ( GetCell( new CellCoordinates( x, y ) ) != null )
		{
			y = (uint)Random.Range( 1, DimensionY );
		}

		OutputCell output = ( new OutputCell( new CellCoordinates( x, y ), ObjectOrientation.Or0, target ) );
		if ( _insert )
		{
			m_outputs.Add( output );
			InsertObject( output );
			Debug.Log( "GridManager: Output added - " + output.Coordinates[0].ToString() + " target " + output.OutputTarget );
		}
		else
		{
			m_futureOutputs.Add( output );
		}
	}

	private void LoadOutputs( List<OutputCell> _outputs, int _numStartingOutputs )
	{
		for ( int i = 0; i < _numStartingOutputs; ++i )
		{
			InsertObject( _outputs[i] );
			Debug.Log( "GridManager: Output added - " + _outputs[i].Coordinates[0].ToString() + " target " + _outputs[i].OutputTarget );
		}
		m_outputs = _outputs.GetRange( 0, _numStartingOutputs );
		m_futureOutputs = _outputs.GetRange( _numStartingOutputs, _outputs.Count - 1 );
		NumTotalOutputs = (uint)_outputs.Count;
		NumStartingOutputs = (uint)_numStartingOutputs;
	}

	private bool AddNextOutput()
	{
		if ( m_futureOutputs.Count > 0 )
		{
			OutputCell output = m_futureOutputs[0];
			InsertObject( output );
			Debug.Log( "GridManager: Output added - " + output.Coordinates[0].ToString() + " target " + output.OutputTarget );
			m_outputs.Add( output );
			m_futureOutputs.RemoveAt( 0 );
			return true;
		}
		return false;
	}

	private List<GridObject> SolveWire( Wire _wire, List<GridObject> _checkedObjects )
	{
		GridObject entry = GetCell( _wire.Entry );
		if ( !_checkedObjects.Contains( entry ) )
		{
			Debug.LogError( "GridManager: SolveWire encountered a wire whose entry has not been solved!" );
			return null;
		}
		_wire.CurrentValues = new int[] { entry.GetValueForCoordinate( _wire.Coordinates[0] ) }; // Assumes wires are 1x1
		GridObject exit = GetCell( _wire.Exit );
		if ( exit != null )
		{
			return new List<GridObject> { exit };
		}
		else
		{
			return null;
		}
	}

	private List<GridObject> SolveGate( Gate _gate, List<GridObject> _checkedObjects )
	{
		List<GridObject> ret = new List<GridObject> { _gate };
		GridObject[] inputs = new GridObject[_gate.Inputs.Length];
		bool ready = true;
		for ( uint i = 0; i < inputs.Length; ++i )
		{
			// Check if all inputs have been solved
			inputs[i] = GetCell( _gate.Inputs[i] );
			if ( !_checkedObjects.Contains( inputs[i] ) )
			{
				ready = false;
				break;
			}
		}
		if ( ready )
		{
			int[] inputValues = new int[inputs.Length];
			for ( uint i = 0; i < inputs.Length; ++i )
			{
				// Get value for coordinate which depepnds on which input we are getting 
				inputValues[i] = inputs[i].GetValueForCoordinate( _gate.GetCoordinateForInput( i ) );
			}
			// Actually perform the operation
			_gate.DoOperation( inputValues );

			ret = new List<GridObject>();
			for ( uint i = 0; i < _gate.Outputs.Length; ++i )
			{
				// Need to solve all outputs next
				GridObject output = GetCell( _gate.Outputs[i] );
				if ( output != null )
				{
					ret.Add( output );
				}
			}
		}
		return ret.Count != 0 ? ret : null;
	}

	private List<GridObject> SolveInput( InputCell _inputCell, List<GridObject> _checkedObjects )
	{
		_inputCell.CurrentValues = new int[] { _inputCell.InputValue };
		GridObject exit = GetCell( _inputCell.Exit );
		if ( exit != null )
		{
			return new List<GridObject> { exit };
		}
		else
		{
			return null;
		}
	}

	private List<GridObject> SolveOutput( OutputCell _outputCell, List<GridObject> _checkedObjects )
	{
		GridObject entry = GetCell( _outputCell.Entry );
		if ( !_checkedObjects.Contains( entry ) )
		{
			Debug.LogError( "GridManager: SolveOutput encountered an output whose entry has not been solved!" );
			return null;
		}
		_outputCell.CurrentValues = new int[] { entry.GetValueForCoordinate( _outputCell.Coordinates[0] ) }; // Assumes outputs are 1x1
		return null;
	}

	private void Solve()
	{
		// Start by assuming all outputs are unsolved
		foreach ( OutputCell output in m_outputs )
		{
			output.Reset();
		}

		// Attempt to solve the grid to determine if you are a good boi.
		Queue<GridObject> objectsToCheck = new Queue<GridObject>();
		List<GridObject> checkedObjects = new List<GridObject>();
		foreach ( InputCell input in m_inputs )
		{
			objectsToCheck.Enqueue( input );
		}

		bool didWork = true;
		while ( didWork )
		{
			didWork = false;
			Queue<GridObject> objectsToCheckNextPass = new Queue<GridObject>();
			while ( objectsToCheck.Count != 0 )
			{
				// Solve all the current objects
				GridObject head = objectsToCheck.Dequeue();
				List<GridObject> next = null;
				switch ( head.ObjectType )
				{
					case GridObjectType.Wire:
					{
						next = SolveWire( (Wire)head, checkedObjects );
						break;
					}
					case GridObjectType.Gate:
					{
						next = SolveGate( (Gate)head, checkedObjects );
						break;
					}
					case GridObjectType.Input:
					{
						next = SolveInput( (InputCell)head, checkedObjects );
						break;
					}
					case GridObjectType.Output:
					{
						next = SolveOutput( (OutputCell)head, checkedObjects );
						break;
					}
				}

				if ( next == null || !( next.Contains( head ) && next.Count == 1 ) )
				{
					// We found some new objects to solve (explicity NOT the same ones as before), and this one has now been solved
					didWork = true;
					checkedObjects.Add( head );
				}

				if ( next != null )
				{
					// Enqueue new objects to solve
					foreach ( GridObject obj in next )
					{
						if ( !objectsToCheckNextPass.Contains( obj ) )
						{
							objectsToCheckNextPass.Enqueue( obj );
						}
					}
				}
			}
			// Prepare to check new objects
			objectsToCheck = new Queue<GridObject>( objectsToCheckNextPass );
		}

		IsSolved = true;
		foreach ( OutputCell output in m_outputs )
		{
			if ( !output.IsCurrentlySatisfied() )
			{
				IsSolved = false;
				break;
			}
		}
	}
}
