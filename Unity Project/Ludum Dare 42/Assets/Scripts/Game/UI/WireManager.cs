using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WireManager : MonoBehaviour
{
	private GridManager m_gridManager;
	private bool m_inWireEditMode;
	private Stack<CellCoordinates> m_passedThrough;
	private VisualGridManager m_visualGridManager;
	private GameplayManager m_gameplayManager;
	private WireVisualManager m_wireVisualManager;
	private bool m_isDragging;
	private float justExited = 0;

	// Use this for initialization
	void Start()
	{
		m_gridManager = FindObjectOfType<GridManager>();
		m_visualGridManager = FindObjectOfType<VisualGridManager>();
		m_gameplayManager = FindObjectOfType<GameplayManager>();
		m_wireVisualManager = FindObjectOfType<WireVisualManager>();
	}

	// Update is called once per frame
	void Update()
	{
		if ( justExited > 0.0f )
		{
			justExited -= Time.deltaTime;
		}

		if ( IsInWireEditMode() )
		{
			Vector2 mousePosition = m_visualGridManager.GetSnapPoint( new Vector2( Input.mousePosition.x, Input.mousePosition.y ) );
			Vector2Int oGridoob = Vector2Int.zero;
			Vector2Int oGrid = Vector2Int.zero;

			bool foundGrid = m_visualGridManager.GetGridCoordinates( mousePosition, ref oGrid, false, true ); // In bounds for dragging
			bool foundGridOOB = m_visualGridManager.GetGridCoordinates( mousePosition, ref oGridoob, false, false )
				&& oGridoob.x >= 0 && oGridoob.x < m_gridManager.DimensionX + 2 && oGridoob.y >= 0 && oGridoob.y < m_gridManager.DimensionY + 2; // Allow oob for start/end

			if ( m_isDragging )
			{
				CellCoordinates cell = new CellCoordinates();
				bool found = false;
				if ( foundGridOOB )
				{
					CellCoordinates oobCell = new CellCoordinates( (uint)oGridoob.x, (uint)oGridoob.y );
					GridObject o = m_gridManager.GetCell( oobCell );
					if ( o != null )
					{
						cell = oobCell;
						found = true;
					}
				}
				if ( !found && foundGrid )
				{
					cell = new CellCoordinates( (uint)oGrid.x, (uint)oGrid.y );
					found = true;
				}

				if ( found )
				{
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
				if ( foundGridOOB )
				{
					CellCoordinates cell = new CellCoordinates( (uint)oGridoob.x, (uint)oGridoob.y );
					// Valid start point, begin
					StartDragging( cell );
					if ( !m_isDragging )
					{
						EndMode();
					}
				}
				else
				{
					EndMode();
				}
			}

			// End Drag/Mode
			if ( Input.GetMouseButtonUp( 0 ) && m_isDragging )
			{
				if ( foundGridOOB )
				{
					CellCoordinates cell = new CellCoordinates( (uint)oGridoob.x, (uint)oGridoob.y );
					// Valid end point, try to commit it (will succed if it is Output or Gate)
					TryCommit( cell );
				}
				else
				{
					// End point invalid, reset path
					ResetMode();
				}
			}

			// Delete
			if ( Input.GetMouseButtonUp( 1 ) && !m_isDragging )
			{
				if ( foundGrid )
				{
					CellCoordinates cell = new CellCoordinates( (uint)oGrid.x, (uint)oGrid.y ); ;
					GridObject o = m_gridManager.GetCell( cell );
					if ( o != null && o.ObjectType == GridObjectType.Wire )
					{
						m_wireVisualManager.ClearWire( cell );
						m_gridManager.ClearCell( cell );
					}
					else
					{
						EndMode();
					}
				}
				else
				{
					EndMode();
				}
			}
		}
	}

	///////////////////////// API /////////////////////////
	public bool IsInWireEditMode()
	{
		return m_inWireEditMode;
	}

	public void ToggleMode()
	{
		if ( justExited <= 0.0f )
		{
			if ( !m_inWireEditMode )
			{
				StartMode();
			}
			else
			{
				EndMode();
			}
		}
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
		justExited = 0.1f;
	}

	public void ResetMode()
	{
		m_passedThrough = new Stack<CellCoordinates>();
		m_isDragging = false;
		m_wireVisualManager.Reset();
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

	private void TryCommit( CellCoordinates _unused )
	{
		CellCoordinates _end = m_passedThrough.Peek();
		GridObject end = m_gridManager.GetCell( _end );
		if ( end == null )
		{
			Debug.Log( "WireManager: Commit attempted on an empty cell " + _end.ToString() + ", ending mode." );
			ResetMode();
			return;
		}
		if ( m_passedThrough.Count < 3 )
		{
			Debug.Log( "WireManager: Commit attempted with too few cells passed through." );
			ResetMode();
			return;
		}

		// Generate wire objects
		List<CellCoordinates> coordinates = new List<CellCoordinates>( m_passedThrough );
		coordinates.Reverse(); // Probably?
		Wire toCreate = new Wire( coordinates.GetRange( 1, coordinates.Count - 2 ).ToArray(), coordinates[0], coordinates[coordinates.Count - 1] );

		// Insert wire into grid
		m_gridManager.InsertObject( toCreate );
		m_gameplayManager.UpdateGiblets( toCreate.Entry );
		m_gameplayManager.UpdateGiblets( toCreate.Exit );

		Debug.Log( "WireManager: Successfully committed at " + _end.ToString() );
		ResetMode();
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

				if ( !m_passedThrough.Contains( _cell ) )
				{
					// Check we haven't been here before on this path
					GridObject cellObject = m_gridManager.GetCell( _cell );
					if ( cellObject == null )
					{
						// Cell is unoccupied, add it
						Debug.Log( "WireManager: Passing through " + _cell.ToString() );
						m_passedThrough.Push( _cell );
					}
					else
					{
						// Need to check it's a valid goal and we arrived in an acceptable way
						switch ( cellObject.ObjectType )
						{
							case GridObjectType.Gate:
							{
								Gate gate = (Gate)cellObject;
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
									Debug.Log( "WireManager: Passing through " + _cell.ToString() );
									m_passedThrough.Push( _cell );
								}
								else
								{
									return;
								}
								break;
							}
							case GridObjectType.Output:
							{
								OutputCell output = (OutputCell)cellObject;
								if ( output.Entry == head )
								{
									// Entry of Output lines up with head, so add Output as end point
									Debug.Log( "WireManager: Passing through " + _cell.ToString() );
									m_passedThrough.Push( _cell );
								}
								else
								{
									;
									return;
								}
								break;
							}
							default:
							{
								return;
							}
						}
					}
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
