using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualWire : MonoBehaviour
{
	public Wire.WireType wireType;
	public ObjectOrientation objectOrientation;

	public void Rotate( bool _clockwise )
	{
		// Update orientation
		int or = (int)objectOrientation;
		or = _clockwise ? or + 1 : or - 1;
		or = or % (int)ObjectOrientation.SIZE;
		objectOrientation = (ObjectOrientation)or;

		// Do visual rotation
	}
}
