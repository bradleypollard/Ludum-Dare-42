using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementDecrementGate : Gate
{
	private int m_amount;

	public IncrementDecrementGate( CellCoordinates _coordinates, ObjectOrientation _orientation, int _amount = 1 )
		: base( FindFootprint( _coordinates, _orientation ), FindInputs( _coordinates, _orientation ), FindOutputs( _coordinates, _orientation ), GateType.IncrementDecrement )
	{
		m_amount = _amount;
	}

	public override void DoOperation( int[] _inputs )
	{
		if ( _inputs.Length != 1 )
		{
			Debug.LogError( "IncrementDecrementGate: Expected 1 input got " + _inputs.Length );
		}

		int result = _inputs[0] + m_amount;
		CurrentValues = new int[] { result };
	}

	private static CellCoordinates[] FindFootprint( CellCoordinates _coordinates, ObjectOrientation _orientation )
	{
		return GridObjectOrientationHelper.Find1x1Footprint( _coordinates, _orientation );
	}

	private static CellCoordinates[] FindInputs( CellCoordinates _coordinates, ObjectOrientation _orientation )
	{
		CellCoordinates input = GridObjectOrientationHelper.Find1x1OffsetLeft( _coordinates, _orientation );
		return new CellCoordinates[] { input };
	}

	private static CellCoordinates[] FindOutputs( CellCoordinates _coordinates, ObjectOrientation _orientation )
	{
		CellCoordinates output = GridObjectOrientationHelper.Find1x1OffsetRight( _coordinates, _orientation );
		return new CellCoordinates[] { output };
	}
}