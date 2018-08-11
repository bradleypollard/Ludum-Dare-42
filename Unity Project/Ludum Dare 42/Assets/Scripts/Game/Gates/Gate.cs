using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GateType
{
	Add,
	Subtract,
	Increment,
	Decrement,
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
		Type = _type;
	}

	public virtual void DoOperation()
	{
		// Implement me in base class
	}
}