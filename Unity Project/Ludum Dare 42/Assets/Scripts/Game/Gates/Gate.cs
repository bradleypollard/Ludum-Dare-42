using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GateType
{
	Add,
	Subtract,
	IncrementDecrement,
	Multiply,
	Divide,
	Cross
}

public abstract class Gate : GridObject
{
	public readonly CellCoordinates[] Inputs;
	public readonly CellCoordinates[] Outputs;
	public readonly GateType Type;

	protected Gate( CellCoordinates[] _coordinates, CellCoordinates[] _inputs, CellCoordinates[] _outputs, GateType _type ) : base( GridObjectType.Gate, _coordinates )
	{
		CurrentValues = new int[_outputs.Length];
		Inputs = _inputs;
		Outputs = _outputs;
		Type = _type;
	}

	public abstract void DoOperation( int[] _inputs );

	public virtual CellCoordinates GetCoordinateForInput( uint input )
	{
		if ( Coordinates.Length != 1 )
		{
			Debug.LogError( "Gate: GetCoordinateForInput using default implementation when num coords > 1! Provide override instead." );
		}
		return Coordinates[0];
	}

	public override int GetValueForCoordinate( CellCoordinates _coordinates )
	{
		for ( int i = 0; i < Outputs.Length; ++i )
		{
			if ( Outputs[i].X == _coordinates.X && Outputs[i].Y == _coordinates.Y )
			{
				return CurrentValues[i];
			}
		}
		Debug.LogError( "Gate: GetValueForCoordinate passed coordinate (" + _coordinates.X + "," + _coordinates.Y + ") which maps to no outputs for this gate." );
		return CurrentValues[0];
	}
}