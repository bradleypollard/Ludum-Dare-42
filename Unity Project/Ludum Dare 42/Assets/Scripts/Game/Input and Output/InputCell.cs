using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputCell : GridObject
{
	public readonly int InputValue;
	public readonly CellCoordinates Exit;

	InputCell( CellCoordinates[] _coordinates, CellCoordinates _exit, int _value ) : base( GridObjectType.Input, _coordinates )
	{
		InputValue = _value;
		Exit = _exit;
	}
}
