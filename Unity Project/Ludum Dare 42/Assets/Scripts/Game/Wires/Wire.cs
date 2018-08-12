using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : GridObject
{
	public readonly CellCoordinates Entry;
	public readonly CellCoordinates Exit;

	public Wire( CellCoordinates[] _coordinates, CellCoordinates _entry, CellCoordinates _exit ) : base( GridObjectType.Wire, _coordinates )
	{
		CurrentValues = new int[1];
		Entry = _entry;
		Exit = _exit;
	}

	public override int GetValueForCoordinate( CellCoordinates _coordinates )
	{
		return CurrentValues[0];
	}
}
