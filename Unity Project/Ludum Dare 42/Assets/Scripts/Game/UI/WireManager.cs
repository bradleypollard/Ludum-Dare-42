using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireManager : MonoBehaviour
{
	private GridManager m_gridManager;
	private bool m_inWireEditMode;
	private Stack<CellCoordinates> m_passedThrough;
	private VisualGridManager m_visualGridManager;
	private bool m_isDragging;

	// Use this for initialization
	void Start()
	{
		m_gridManager = FindObjectOfType<GridManager>();
		m_visualGridManager = FindObjectOfType<VisualGridManager>();
	}

	// Update is called once per frame
	void Update()
	{
		if ( IsInWireEditMode() )
		{
			Vector2 mousePosition = m_visualGridManager.GetSnapPoint( new Vector2( Input.mousePosition.x, Input.mousePosition.y ) );
			Vector2Int oGrid = Vector2Int.zero;
			bool foundGrid = m_visualGridManager.GetGridCoordinates( mousePosition, ref oGrid, false, false )
				&& oGrid.x >= 0 && oGrid.x < m_gridManager.DimensionX + 2 && oGrid.y >= 0 && oGrid.y < m_gridManager.DimensionY + 2;

			if ( m_isDragging )
			{
				if ( foundGrid )
				{
					CellCoordinates cell = new CellCoordinates( (uint)oGrid.x, (uint)oGrid.y );
					PassThroughCell( cell );
					string log = "WireManager: Current path - ";
					foreach ( CellCoordinates x in m_passedThrough.ToArray() )
					{
						log += x.ToString() + " ";
					}
					// Debug.Log( log );
				}
			}

			// Start drag
			if ( Input.GetMouseButtonDown( 0 ) && !m_isDragging )
			{
				if ( foundGrid )
				{
					CellCoordinates cell = new CellCoordinates( (uint)oGrid.x, (uint)oGrid.y );
					// Valid start point, begin
					StartDragging( cell );
				}
			}

			// End Drag/Mode
			if ( Input.GetMouseButtonUp( 0 ) && m_isDragging )
			{
				if ( foundGrid )
				{
					CellCoordinates cell = new CellCoordinates( (uint)oGrid.x, (uint)oGrid.y );
					// Valid end point, try to commit it (will succed if it is Output or Gate)
					TryCommit( cell );
				}
				else
				{
					// End point invalid, end mode
					EndMode();
				}
			}


			if ( Input.GetMouseButtonUp( 1 ) )
			{
				// Right click, exit mode
				EndMode();
			}

		}
	}

	///////////////////////// API /////////////////////////
	public bool IsInWireEditMode()
	{
		return m_inWireEditMode;
	}

	public void StartMode()
	{
		m_inWireEditMode = true;
		m_passedThrough = new Stack<CellCoordinates>();
		m_isDragging = false;
	}

	public void EndMode()
	{
		m_inWireEditMode = false;
	}

	public List<CellCoordinates> GetCurrentPath()
	{
		List<CellCoordinates> ret = new List<CellCoordinates>( m_passedThrough );
		ret.Reverse();
		return ret;
	}

	///////////////////////// Helpers /////////////////////////
	private void StartDragging( CellCoordinates _start )
	{

		GridObject gridObject = m_gridManager.GetCell( _start );
		if ( gridObject != null && ( gridObject.ObjectType == GridObjectType.Gate || gridObject.ObjectType == GridObjectType.Input ) )
		{
			Debug.Log( "WireManager: Starting drag from " + _start.ToString() );
			m_passedThrough.Push( _start );
			m_isDragging = true;
		}
	}

	private void TryCommit( CellCoordinates _end )
	{
		GridObject end = m_gridManager.GetCell( _end );
		if ( end == null )
		{
			Debug.Log( "WireManager: Commit attempted on an empty cell " + _end.ToString() + ", ending mode." );
			EndMode();
			return;
		}

		// Check if we arrived at a destination in a valid way
		CellCoordinates head = m_passedThrough.Peek();
		switch ( end.ObjectType )
		{
			case GridObjectType.Gate:
			{
				Gate gate = (Gate)end;
				bool isValid = false;
				foreach ( CellCoordinates input in gate.Inputs )
				{
					if ( input == head )
					{
						isValid = true;
						break;
					}
				}
				if ( isValid )
				{
					// Head lines up with an input for this gate so add Gate as end point
					m_passedThrough.Push( _end );
				}
				else
				{
					Debug.Log( "WireManager: Commit attempted on Gate at " + _end.ToString() + " but did not align with the Inputs." );
					EndMode();
					return;
				}
				break;
			}
			case GridObjectType.Output:
			{
				OutputCell output = (OutputCell)end;
				if ( output.Entry == head )
				{
					// Entry of Output lines up with head, so add Output as end point
					m_passedThrough.Push( _end );
				}
				else
				{
					Debug.Log( "WireManager: Commit attempted on Output at " + _end.ToString() + " but did not align with the Entry." );
					EndMode();
					return;
				}
				break;
			}
			default:
			{
				Debug.Log( "WireManager: Commit attempted on a cell that was neither a Gate, nor an Output, ending mode." );
				EndMode();
				return;
			}
		}

		// Generate wire objects
		List<Wire> wiresToCreate = new List<Wire>();
		List<CellCoordinates> coordinates = new List<CellCoordinates>( m_passedThrough );
		coordinates.Reverse(); // Probably?
		for ( int i = 2; i < coordinates.Count; ++i )
		{
			wiresToCreate.Add( new Wire( new CellCoordinates[]{ coordinates[i - 1] }, coordinates[i - 2], coordinates[i] ) );
		}

		// Insert wires into grid
		foreach ( Wire wire in wiresToCreate )
		{
			m_gridManager.InsertObject( wire );
		}

		Debug.Log( "WireManager: Successfully committed at " + _end.ToString() );
		EndMode();
	}

	private void PassThroughCell( CellCoordinates _cell )
	{
		if ( m_passedThrough.Peek() != _cell )
		{
			CellCoordinates head = m_passedThrough.Pop();
			if ( _cell != head && IsAdjacentTo( _cell, head ) )
			{
				// Wait till we move, and only consider if it is adjacent
				if ( m_passedThrough.Count >= 1 )
				{
					CellCoordinates prev = m_passedThrough.Pop();
					if ( _cell == prev )
					{
						// Early out if we went backwards
						Debug.Log( "WireManager: Going backwards to " + _cell.ToString() );
						m_passedThrough.Push( prev );
						return;
					}
					m_passedThrough.Push( prev );
				}
				else
				{
					// This is the first move, assert that we head onto a valid OUTPUT of our starting gate
					GridObject gridObject = m_gridManager.GetCell( head );
					switch ( gridObject.ObjectType )
					{
						case GridObjectType.Input:
						{
							InputCell input = (InputCell)gridObject;
							if ( input.Exit != _cell )
							{
								m_passedThrough.Push( head ); // Put the head back on and early out, this is not the exit cell
								return;
							}
							break;
						}
						case GridObjectType.Gate:
						{
							Gate gate = (Gate)gridObject;
							bool isValid = false;
							foreach ( CellCoordinates output in gate.Outputs )
							{
								if ( output == _cell )
								{
									isValid = true;
								}
							}
							if ( !isValid )
							{
								m_passedThrough.Push( head ); // We are not over any valid output cell, put the head back on and early out
								return;
							}
							break;
						}
						default:
						{
							Debug.LogError( "WireManager: PassThrough found starting position that wasn't a Gate or Input." );
							m_passedThrough.Push( head ); // Put the head back on and early out
							return;
						}
					}
				}
				m_passedThrough.Push( head ); // Put the head back on, moving forwards

				GridObject cellObject = m_gridManager.GetCell( _cell );
				if ( cellObject == null )
				{
					Debug.Log( "WireManager: Passing through " + _cell.ToString() );
					// Only add this cell if it is unoccupied
					m_passedThrough.Push( _cell );
				}
				return;
			}
			m_passedThrough.Push( head ); // Put the head back on, no new additions
		}
	}

	private bool IsAdjacentTo( CellCoordinates _cell, CellCoordinates _other )
	{
		int xDif = Mathf.Abs( (int)_cell.X - (int)_other.X );
		int yDif = Mathf.Abs( (int)_cell.Y - (int)_other.Y );
		return xDif + yDif <= 1;
	}
}
