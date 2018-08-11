using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : GridObject
{
	public readonly CellCoordinates Entry;
	public readonly CellCoordinates Exit;

	Wire( CellCoordinates[] _coordinates, CellCoordinates _entry, CellCoordinates _exit ) : base( GridObjectType.Wire, _coordinates )
	{
		Entry = _entry;
		Exit = _exit;
	}
}
