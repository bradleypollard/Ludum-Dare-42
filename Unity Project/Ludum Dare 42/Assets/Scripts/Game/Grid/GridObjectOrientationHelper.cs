using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObjectOrientationHelper
{
	public static CellCoordinates[] Find1x1Footprint( CellCoordinates _coordinates, ObjectOrientation _orientation )
	{
		return new CellCoordinates[] { _coordinates };
	}

	public static CellCoordinates[] Find2x1Footprint( CellCoordinates _coordinates, ObjectOrientation _orientation )
	{
		CellCoordinates coords = Find1x1OffsetDown( _coordinates, _orientation );
		return new CellCoordinates[] { _coordinates, coords };
	}

	public static CellCoordinates Find1x1OffsetRight( CellCoordinates _coordinates, ObjectOrientation _orientation )
	{
		CellCoordinates coords = _coordinates;

		switch ( _orientation )
		{
			case ObjectOrientation.Or0:
			{
				coords.X += 1;
				break;
			}
			case ObjectOrientation.Or90:
			{
				coords.Y -= 1;
				break;
			}
			case ObjectOrientation.Or180:
			{
				coords.X -= 1;
				break;
			}
			case ObjectOrientation.Or270:
			{
				coords.Y += 1;
				break;
			}
		}

		return coords;
	}

	public static CellCoordinates Find1x1OffsetDown( CellCoordinates _coordinates, ObjectOrientation _orientation )
	{
		int orientation = (int)_orientation;
		orientation += 1;
		orientation = orientation % (int)ObjectOrientation.SIZE;

		return Find1x1OffsetRight( _coordinates, (ObjectOrientation)orientation );
	}

	public static CellCoordinates Find1x1OffsetLeft( CellCoordinates _coordinates, ObjectOrientation _orientation )
	{
		int orientation = (int)_orientation;
		orientation += 2;
		orientation = orientation % (int)ObjectOrientation.SIZE;

		return Find1x1OffsetRight( _coordinates, (ObjectOrientation)orientation );
	}

	public static CellCoordinates Find1x1OffsetUp( CellCoordinates _coordinates, ObjectOrientation _orientation )
	{
		int orientation = (int)_orientation;
		orientation += 3;
		orientation = orientation % (int)ObjectOrientation.SIZE;

		return Find1x1OffsetRight( _coordinates, (ObjectOrientation)orientation );
	}
}
