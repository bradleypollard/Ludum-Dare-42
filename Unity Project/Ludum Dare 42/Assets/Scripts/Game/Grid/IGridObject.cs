using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellObjectType
{
	Wire,
	Gate
}

public class IGridObject
{
	public CellObjectType Type;
}
