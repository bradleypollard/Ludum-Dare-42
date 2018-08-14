using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplyGate : Gate
{
	public MultiplyGate( CellCoordinates _coordinates, ObjectOrientation _orientation )
		: base( FindFootprint( _coordinates, _orientation ), FindInputs( _coordinates, _orientation ), FindOutputs( _coordinates, _orientation ), GateType.Multiply )
	{
	}

	public override void DoOperation( int[] _inputs, bool[] _readyInputs, int numReadyInputs )
	{
		if ( _inputs.Length != 2 || numReadyInputs != _inputs.Length )
		{
			Debug.LogError( "MultiplyGate: Expected 2 inputs got " + numReadyInputs );
		}

		int result = _inputs[0] * _inputs[1];
		CurrentValues = new int[] { result };
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
		CellCoordinates output = GridObjectOrientationHelper.Find1x1OffsetRight( _coordinates, _orientation );
		return new CellCoordinates[] { output };
	}
}