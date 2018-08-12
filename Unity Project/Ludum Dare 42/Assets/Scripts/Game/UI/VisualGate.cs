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
}
