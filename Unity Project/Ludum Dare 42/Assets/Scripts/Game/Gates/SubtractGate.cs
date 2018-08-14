using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtractGate : Gate
{
	public SubtractGate( CellCoordinates _coordinates, ObjectOrientation _orientation )
		: base( FindFootprint( _coordinates, _orientation ), FindInputs( _coordinates, _orientation ), FindOutputs( _coordinates, _orientation ), GateType.Subtract )
	{
	}

	public override void DoOperation( int[] _inputs, bool[] _readyInputs, int numReadyInputs )
	{
		if ( _inputs.Length != 2 || numReadyInputs != _inputs.Length )
		{
			Debug.LogError( "SubtractionGate: Expected 2 inputs got " + numReadyInputs );
		}

		int result = _inputs[0] - _inputs[1];
		CurrentValues = new int[] { result };
	}

	public override CellCoordinates GetCoordinateForInput( uint input )
	{
		if ( input == 0 )
		{
			return Coordinates[0];
		}
		else
		{
			return Coordinates[1];
		}
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