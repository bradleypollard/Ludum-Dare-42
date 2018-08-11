using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VisualGridManager : MonoBehaviour
{
    //Bunch of Public Variables cause I'm lazy
    public Vector2 startPos, endPos;
    public int gridWidth, gridHeight;
    public bool isDebug;

    public Vector2 GetSnapPoint(Vector2 _position)
    {
        if (_position.x > startPos.x && _position.y > startPos.y && _position.x < endPos.x && _position.y < endPos.y)
        {
            Vector2 gridSize = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);
            Vector2 squareSize = new Vector2(gridSize.x / gridWidth, gridSize.y / gridHeight);

            Vector2 closestSnapPoint = ((_position - startPos) / squareSize);
            Vector2 flooredSnapPoint = new Vector2(Mathf.Floor(closestSnapPoint.x), Mathf.Floor(closestSnapPoint.y));
            flooredSnapPoint *= squareSize;

            flooredSnapPoint += (squareSize * 0.5f);

            return flooredSnapPoint + startPos;
        }

        return _position;
    }

    public bool GetGridCoordinates(Vector2 _position, ref Vector2Int o_position)
    {
        if (_position.x > startPos.x && _position.y > startPos.y && _position.x < endPos.x && _position.y < endPos.y)
        {
            Vector2 gridSize = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);
            Vector2 squareSize = new Vector2(gridSize.x / gridWidth, gridSize.y / gridHeight);

            Vector2 closestSnapPoint = ((_position - startPos) / squareSize);
            o_position = new Vector2Int(Mathf.FloorToInt(closestSnapPoint.x), Mathf.FloorToInt(closestSnapPoint.y));
            return true;
        }

        return false;
    }

    public void Update()
    {
        if (isDebug)
        {
            Vector2 gridSize = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);
            Vector2 squareSize = new Vector2(gridSize.x / gridWidth, gridSize.y / gridHeight);

            //Draw HoriLines
            for (float y = startPos.y; y <= endPos.y; y += squareSize.y)
            {
                Debug.DrawLine(new Vector3(startPos.x, y, 5), new Vector3(endPos.x, y, 5));
            }

            for (float x = startPos.x; x <= endPos.x; x += squareSize.x)
            {
                Debug.DrawLine(new Vector3(x, startPos.y, 5), new Vector3(x, endPos.y, 5));
            }
        }
    }
}
