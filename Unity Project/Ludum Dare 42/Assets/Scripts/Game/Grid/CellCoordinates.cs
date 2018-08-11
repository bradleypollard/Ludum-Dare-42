using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CellCoordinates
{
	public uint X;
	public uint Y;

	public static CellCoordinates GetCoordinatesForSide( CellCoordinates _coordinates, CellSide _side )
	{
		CellCoordinates res = _coordinates;
		switch ( _side )
		{
			case CellSide.Top:
			{
				res.Y += 1;
				break;
			}
			case CellSide.Right:
			{
				res.X += 1;
				break;
			}
			case CellSide.Bottom:
			{
				res.Y -= 1;
				break;
			}
			case CellSide.Left:
			{
				res.X -= 1;
				break;
			}
		}
		return res;
	}
}
