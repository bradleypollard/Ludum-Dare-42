using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireManager : MonoBehaviour
{
	private GridManager m_gridManager;
	private bool m_inWireEditMode;
	private CellCoordinates m_previous;
	private CellCoordinates m_current;
	private List<Wire> m_wiresToCreate;

	// Use this for initialization
	void Start()
	{
		m_gridManager = FindObjectOfType<GridManager>();
	}

	// Update is called once per frame
	void Update()
	{
		if ( Input.GetMouseButtonUp( 1 ) )
		{
			EndMode();
		}
	}

	///////////////////////// API /////////////////////////
	public bool IsInWireEditMode()
	{
		return m_inWireEditMode;
	}

	public void StartMode( CellCoordinates _cell )
	{
		m_inWireEditMode = true;
		m_current = _cell;
		m_previous = _cell;
		m_wiresToCreate = new List<Wire>();
	}

	public void EndMode()
	{
		m_inWireEditMode = false;
	}

	public void Commit()
	{
		foreach ( Wire wire in m_wiresToCreate )
		{
			m_gridManager.InsertObject( wire );
		}
		EndMode();
	}

	public void PassThroughCell( CellCoordinates _cell )
	{
		if ( m_current != _cell )
		{
			if ( m_previous != m_current )
			{
				AddWire( m_previous, m_current, _cell );
			}
			m_previous = m_current;
			m_current = _cell;
		}
	}

	///////////////////////// Helpers /////////////////////////
	private void AddWire( CellCoordinates _prev, CellCoordinates _current, CellCoordinates _next )
	{
		// GridObject nextObj = m_gridManager.GetCell( _next );
		GridObject currentObj = m_gridManager.GetCell( _current );

		if ( currentObj == null )  
		{
			m_wiresToCreate.Add( new Wire( new CellCoordinates[] { _current }, _prev, _next ) );
		}
		// Is this an error otherwise? Handle cross!
	}
}
