using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CellCoordinates
{
	public uint X;
	public uint Y;

    public CellCoordinates (uint _x, uint _y)
    {
        X = _x;
        Y = _y;
    }

	public override string ToString()
	{
		return "(" + X + "," + Y + ")";
	}

	public static bool operator ==( CellCoordinates lhs, CellCoordinates rhs )
	{
		return lhs.Equals( rhs );
	}

	public static bool operator !=( CellCoordinates lhs, CellCoordinates rhs )
	{
		return !lhs.Equals( rhs );
	}

	public override bool Equals( object obj )
	{
		if ( obj is CellCoordinates )
		{
			return ( ( (CellCoordinates)obj ).X == this.X &&
					   ( (CellCoordinates)obj ).Y == this.Y );
		}
		else
		{
			return false;
		}
	}
}
