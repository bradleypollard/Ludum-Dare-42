﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Wire : GridObject
{
	public enum WireType
	{
		Straight,
		Turn
	}

	public readonly CellCoordinates Entry;
	public readonly CellCoordinates Exit;
	public readonly WireType Type;

	protected Wire( CellCoordinates[] _coordinates, CellCoordinates _entry, CellCoordinates _exit, WireType _wire ) : base( GridObjectType.Wire, _coordinates )
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
