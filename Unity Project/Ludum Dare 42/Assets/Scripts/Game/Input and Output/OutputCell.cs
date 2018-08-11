using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputCell : GridObject
{
	public readonly int OutputTarget;
	public readonly CellCoordinates Entry;

	OutputCell( CellCoordinates[] _coordinates, CellCoordinates _entry, int _target ) : base( GridObjectType.Input, _coordinates )
	{
		OutputTarget = _target;
		Entry = _entry;
	}
}