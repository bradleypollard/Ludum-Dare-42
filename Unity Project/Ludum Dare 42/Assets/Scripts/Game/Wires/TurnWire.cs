using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnWire : Wire
{
	public TurnWire( CellCoordinates _coordinates, ObjectOrientation _orientation )
		: base( FindFootprint( _coordinates, _orientation ), FindEntry( _coordinates, _orientation ), FindExit( _coordinates, _orientation ), WireType.Straight )
	{
	}

	private static CellCoordinates[] FindFootprint( CellCoordinates _coordinates, ObjectOrientation _orientation )
	{
		return GridObjectOrientationHelper.Find1x1Footprint( _coordinates, _orientation );
	}

	private static CellCoordinates FindEntry( CellCoordinates _coordinates, ObjectOrientation _orientation )
	{
		return GridObjectOrientationHelper.Find1x1OffsetLeft( _coordinates, _orientation );

	}
	private static CellCoordinates FindExit( CellCoordinates _coordinates, ObjectOrientation _orientation )
	{
		return GridObjectOrientationHelper.Find1x1OffsetDown( _coordinates, _orientation );


	}
}
