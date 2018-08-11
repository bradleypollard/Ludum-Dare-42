using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
	public int Dimension;

	private IGridObject[,] m_grid;

	// Use this for initialization
	void Start()
	{
		// Generate grid 2 cells larger than required for inputs and outputs. 
		// Player can only edit the centre Dimension x Dimension grid
		m_grid = new IGridObject[Dimension + 2, Dimension + 2];
	}

	// Update is called once per frame
	void Update()
	{
		// TODO: Parse grid, work out input/outputs
	}

	// API
	public void Insert( int _x, int _y, IGridObject _object )
	{
		m_grid[_x, _y] = _object;
	}

}
