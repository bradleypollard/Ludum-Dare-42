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
			bool foundGrid = m_visualGridManager.GetGridCoordinates( mousePosition, ref oGrid, false );

			if ( m_isDragging )
			{
				if ( foundGrid )
				{
					CellCoordinates cell = new CellCoordinates( (uint)oGrid.x, (uint)oGrid.y );
					PassThroughCell( cell );
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
			else if ( Input.GetMouseButtonUp( 0 ) )
			{
				// Click invalid, end mode
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

	///////////////////////// Helpers /////////////////////////
	private void StartDragging( CellCoordinates _start )
	{

		GridObject gridObject = m_gridManager.GetCell( _start );
		if ( gridObject.ObjectType == GridObjectType.Gate || gridObject.ObjectType == GridObjectType.Input )
		{
			m_passedThrough.Push( _start );
			m_isDragging = true;
		}
	}

	private void TryCommit( CellCoordinates _end )
	{
		GridObject end = m_gridManager.GetCell( _end );

		switch ( end.ObjectType )
		{
			case GridObjectType.Gate:
			{
				break;
			}
			case GridObjectType.Output:
			{
				break;
			}
			default:
			{
				Debug.Log( "WireManager: Commit attempted on a cell that was neither a Gate, nor an Output, ending mode." );
				EndMode();
				return;
			}
		}

		List<Wire> wiresToCreate = new List<Wire>();
		foreach ( Wire wire in wiresToCreate )
		{
			m_gridManager.InsertObject( wire );
		}
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
						m_passedThrough.Push( prev );
						return;
					}
				}
				m_passedThrough.Push( head ); // Put the head back on, moving forwards

				GridObject cellObject = m_gridManager.GetCell( _cell );
				if ( cellObject == null )
				{
					// Only add this cell if it is unoccupied
					m_passedThrough.Push( _cell );
				}
			}
		}
	}

	private bool IsAdjacentTo( CellCoordinates _cell, CellCoordinates _other )
	{
		return ( Mathf.Abs( _cell.X - _other.X ) <= 1 && Mathf.Abs( _cell.Y - _other.Y ) <= 1 );
	}
}
