using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : IGridObject
{
	public CellSide EntrySide;
	public CellSide ExitSide;

	public CellCoordinates GetExitCoordinates( CellCoordinates _pos )
	{
		return CellCoordinates.GetCoordinatesForSide( _pos, ExitSide );
	}

	public CellCoordinates GetEntryCoordinates( CellCoordinates _pos )
	{
		return CellCoordinates.GetCoordinatesForSide( _pos, EntrySide );
	}
}
