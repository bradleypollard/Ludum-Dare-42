using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualWire : MonoBehaviour
{
    public Wire.WireType wireType;

    private Vector2 spawnLocation;
    public Vector2 GetSpawnLocation() { return spawnLocation; }
    public void Start()
    {
        spawnLocation = GetComponent<RectTransform>().anchoredPosition;
    }
}
