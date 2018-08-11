using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossGate : Gate
{
	public CrossGate( CellCoordinates _coordinates, ObjectOrientation _orientation )
		: base( FindFootprint( _coordinates, _orientation ), FindInputs( _coordinates, _orientation ), FindOutputs( _coordinates, _orientation ), GateType.Cross )
	{
	}

	public override void DoOperation( int[] _inputs )
	{
		if ( _inputs.Length != 2 )
		{
			Debug.LogError( "CrossGate: Expected 2 input got " + _inputs.Length );
		}
		CurrentValues = new int[] { _inputs[0], _inputs[1] };
	}

	private static CellCoordinates[] FindFootprint( CellCoordinates _coordinates, ObjectOrientation _orientation )
	{
		return GridObjectOrientationHelper.Find1x1Footprint( _coordinates, _orientation );
	}

	private static CellCoordinates[] FindInputs( CellCoordinates _coordinates, ObjectOrientation _orientation )
	{
		CellCoordinates input1 = GridObjectOrientationHelper.Find1x1OffsetLeft( _coordinates, _orientation );
		CellCoordinates input2 = GridObjectOrientationHelper.Find1x1OffsetUp( _coordinates, _orientation );
		return new CellCoordinates[] { input1, input2 };
	}

	private static CellCoordinates[] FindOutputs( CellCoordinates _coordinates, ObjectOrientation _orientation )
	{
		CellCoordinates output1 = GridObjectOrientationHelper.Find1x1OffsetRight( _coordinates, _orientation );
		CellCoordinates output2 = GridObjectOrientationHelper.Find1x1OffsetDown( _coordinates, _orientation );
		return new CellCoordinates[] { output1, output2 };
	}
}