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
		InputValue = _value;
		Exit = GridObjectOrientationHelper.Find1x1OffsetRight( _coordinates, _orientation );
	}

	private static CellCoordinates[] FindFootprint( CellCoordinates _coordinates, ObjectOrientation _orientation )
	{
		return GridObjectOrientationHelper.Find1x1Footprint( _coordinates, _orientation );
	}
}
