using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridObjectType
{
	Wire,
	Gate,
	Input,
	Output
}

public class GridObject
{
	public readonly GridObjectType ObjectType;
	public readonly CellCoordinates[] Coordinates; // All of the coordinates we occupy

	protected GridObject( GridObjectType _type, CellCoordinates[] _coordinates )
	{
		ObjectType = _type;
		Coordinates = _coordinates;
	}
}
