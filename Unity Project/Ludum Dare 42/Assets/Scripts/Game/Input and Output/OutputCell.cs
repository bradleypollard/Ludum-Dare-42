using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputCell : GridObject
{
	public readonly int OutputTarget;
	public readonly CellCoordinates Entry;

	public OutputCell( CellCoordinates _coordinates, ObjectOrientation _orientation, int _target )
		: base( GridObjectType.Output, FindFootprint( _coordinates, _orientation ) )
	{
		CurrentValues = new int[1];
		OutputTarget = _target;
		Entry = GridObjectOrientationHelper.Find1x1OffsetLeft( _coordinates, _orientation );
	}

	public void Reset()
	{
		CurrentValues[0] = 0;
	}

	public bool IsCurrentlySatisfied()
	{
		return OutputTarget == CurrentValues[0];
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