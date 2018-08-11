using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputCell : GridObject
{
	public readonly int InputValue;
	public readonly CellCoordinates Exit;

	public InputCell( CellCoordinates _coordinates, ObjectOrientation _orientation, int _value )
		: base( GridObjectType.Input, FindFootprint( _coordinates, _orientation ) )
	{
		CurrentValues = new int[1];
		InputValue = _value;
		Exit = GridObjectOrientationHelper.Find1x1OffsetRight( _coordinates, _orientation );
	}

	public override int GetValueForCoordinate( CellCoordinates _coordinates )
	{
		return CurrentValues[0];
	}

	private static CellCoordinates[] FindFootprint( CellCoordinates _coordinates, ObjectOrientation _orientation )
	{
		return GridObjectOrientationHelper.Find1x1Footprint( _coordinates, _orientation );
	}
}
