using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualGate : MonoBehaviour
{
	public GateType gateType;
	public int value;
	public ObjectOrientation objectOrientation;

	private Vector2 spawnLocation;
	public Vector2 GetSpawnLocation() { return spawnLocation; }

	public void Start()
	{
		spawnLocation = GetComponent<RectTransform>().anchoredPosition;
	}


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