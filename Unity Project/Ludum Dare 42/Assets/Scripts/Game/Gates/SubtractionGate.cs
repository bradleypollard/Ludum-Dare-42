using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtractionGate : Gate
{
    public SubtractionGate( CellCoordinates _coordinates, ObjectOrientation _orientation )
		: base( FindFootprint( _coordinates, _orientation ), FindInputs( _coordinates, _orientation ), FindOutputs( _coordinates, _orientation ), GateType.Subtract )
	{
	}

	private static CellCoordinates[] FindFootprint( CellCoordinates _coordinates, ObjectOrientation _orientation )
	{
		return GridObjectOrientationHelper.Find2x1Footprint( _coordinates, _orientation );
	}

	private static CellCoordinates[] FindInputs( CellCoordinates _coordinates, ObjectOrientation _orientation )
	{
		CellCoordinates[] footprint = GridObjectOrientationHelper.Find2x1Footprint( _coordinates, _orientation );

		CellCoordinates[] inputs = new CellCoordinates[footprint.Length];
		for ( uint i = 0; i < footprint.Length; ++i )
		{
			inputs[i] = GridObjectOrientationHelper.Find1x1OffsetLeft( footprint[i], _orientation );
		}

		return inputs;
	}
	private static CellCoordinates[] FindOutputs( CellCoordinates _coordinates, ObjectOrientation _orientation )
	{
		CellCoordinates output = GridObjectOrientationHelper.Find1x1OffsetRight( _coordinates, _orientation );
		return new CellCoordinates[] { output };
	}
}