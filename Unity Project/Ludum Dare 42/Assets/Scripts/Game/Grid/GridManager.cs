using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
	public int DimensionX, DimensionY;

	private GridObject[,] m_grid;

	// Use this for initialization
	void Start()
	{
		ClearGrid();
	}

	// Update is called once per frame
	void Update()
	{
		// TODO: Parse grid, work out input/outputs
	}

	// API
	public void InsertObject( GridObject _object )
	{
		// Inserts the given GridObject into all the coordinates it occupies
		for ( uint i = 0; i < _object.Coordinates.Length; ++i )
		{
			if ( m_grid[_object.Coordinates[i].X, _object.Coordinates[i].Y] != null )
			{
				Debug.LogError( "Inserting a GridObject into an occupied cell." );
			}

            Debug.Log("Inserting a GridObject into " + _object.Coordinates[i].X.ToString() + ", " + _object.Coordinates[i].Y.ToString());
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

	// Helpers
	private void ClearGrid()
	{
		// Generate grid 2 cells larger than required for inputs and outputs. 
		// Player can only edit the centre Dimension x Dimension grid
		m_grid = new GridObject[DimensionX + 2, DimensionY + 2];
	}
}
