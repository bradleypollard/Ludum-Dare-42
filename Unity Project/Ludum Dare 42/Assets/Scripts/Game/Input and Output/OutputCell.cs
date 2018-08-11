using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputCell : GridObject
{
	public readonly int OutputTarget;
	public readonly CellCoordinates Entry;

	OutputCell( CellCoordinates _coordinates, ObjectOrientation _orientation, int _target )
		: base( GridObjectType.Input, FindFootprint( _coordinates, _orientation ) )
	{
		OutputTarget = _target;
		Entry = GridObjectOrientationHelper.Find1x1OffsetLeft( _coordinates, _orientation );
	}

	private static CellCoordinates[] FindFootprint( CellCoordinates _coordinates, ObjectOrientation _orientation )
	{
		return GridObjectOrientationHelper.Find1x1Footprint( _coordinates, _orientation );
	}
}