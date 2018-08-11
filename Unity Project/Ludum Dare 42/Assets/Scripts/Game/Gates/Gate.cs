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
	Divide
}

public class Gate : GridObject
{
	public readonly CellCoordinates[] Inputs;
	public readonly CellCoordinates[] Outputs;
	public readonly GateType GateType;

	protected Gate( CellCoordinates[] _coordinates, CellCoordinates[] _inputs, CellCoordinates[] _outputs, GateType _type ) : base( GridObjectType.Gate, _coordinates )
	{
		GateType = _type;
	}

	public virtual void DoOperation()
	{
		// Implement me in base class
	}
}