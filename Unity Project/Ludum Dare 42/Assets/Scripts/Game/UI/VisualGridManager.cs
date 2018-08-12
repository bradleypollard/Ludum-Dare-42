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

    private UICanvasFixer uiCanvasFixer;
    private Texture2D line;

    private void Start()
    {
        uiCanvasFixer = FindObjectOfType<UICanvasFixer>();

        line = new Texture2D(1, 1);
        line.SetPixel(0, 0, Color.white);
        line.Apply();
    }

    public Vector2 GetSnapPoint(Vector2 _position, VisualGate _visualGate = null)
    {
        Vector2 actualPos = uiCanvasFixer.ScreenPosToCanvas(_position);

        if (actualPos.x > startPos.x && actualPos.y > startPos.y && actualPos.x < endPos.x && actualPos.y < endPos.y)
        {
            if(_visualGate != null)
            {
                //Check that this gridObject won't overlap anything
                if(!IsGridObjectValid(actualPos, _visualGate, false))
                {
                    return actualPos;
                }
            }

            Vector2 gridSize = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);
            Vector2 squareSize = new Vector2(gridSize.x / gridWidth, gridSize.y / gridHeight);

            Vector2 closestSnapPoint = ((actualPos - startPos) / squareSize);
            Vector2 flooredSnapPoint = new Vector2(Mathf.Floor(closestSnapPoint.x), Mathf.Floor(closestSnapPoint.y));
            flooredSnapPoint *= squareSize;

            flooredSnapPoint += (squareSize * 0.5f);

            return flooredSnapPoint + startPos;
        }

        return actualPos;
    }

    public Vector2 GetLocalPos(Vector2 _position)
    {
        Vector2 actualPos = uiCanvasFixer.ScreenPosToCanvas(_position);
        return actualPos;
    }

    public bool GetGridCoordinates(Vector2 _position, ref Vector2Int o_position, bool _convertToCanvas, bool _careIsInGrid = true)
    {
        Vector2 actualPos = _convertToCanvas ? uiCanvasFixer.ScreenPosToCanvas(_position) : _position;

        if (!_careIsInGrid || (actualPos.x > startPos.x && actualPos.y > startPos.y && actualPos.x < endPos.x && actualPos.y < endPos.y))
        {
            actualPos -= startPos;

            Vector2 gridSize = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);
            Vector2 squareSize = new Vector2(gridSize.x / gridWidth, gridSize.y / gridHeight);

            o_position = new Vector2Int(Mathf.FloorToInt(actualPos.x / squareSize.x) + 1, Mathf.FloorToInt(actualPos.y / squareSize.y) + 1);

            return true;
        }

        return false;
    }

    public Vector2 GetScreenFromGrid(CellCoordinates _coordinates)
    {
        Vector2 gridSize = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);
        Vector2 squareSize = new Vector2(gridSize.x / gridWidth, gridSize.y / gridHeight);

        return startPos + (new Vector2((int)_coordinates.X - 1, (int)_coordinates.Y - 1) * squareSize) + (squareSize * 0.5f);
    }

    public bool IsGridObjectValid(Vector2 _position, VisualGate _gate, bool _convertToCanvas)
    {
        GridObject gridObject = null;
        Vector2Int oGrid = Vector2Int.zero;

        if (GetGridCoordinates(_position, ref oGrid, _convertToCanvas))
        {
            CellCoordinates cell = new CellCoordinates((uint)oGrid.x, (uint)oGrid.y);

            switch (_gate.gateType)
            {
                case GateType.Add:
                    {
                        gridObject = new AddGate(cell, _gate.objectOrientation);
                        break;
                    }
                case GateType.Subtract:
                    {
                        gridObject = new SubtractGate(cell, _gate.objectOrientation);
                        break;
                    }
                case GateType.Multiply:
                    {
                        gridObject = new MultiplyGate(cell, _gate.objectOrientation);
                        break;
                    }
                case GateType.Divide:
                    {
                        gridObject = new DivideGate(cell, _gate.objectOrientation);
                        break;
                    }
                case GateType.IncrementDecrement:
                    {
                        gridObject = new IncrementDecrementGate(cell, _gate.objectOrientation);
                        break;
                    }
                case GateType.Cross:
                    {
                        gridObject = new CrossGate(cell, _gate.objectOrientation);
                        break;
                    }
            }

            foreach(CellCoordinates coordinates in gridObject.Coordinates)
            {
                if(coordinates.X < 1 || coordinates.Y < 1 || coordinates.X > gridWidth || coordinates.Y > gridHeight)
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    public void OnGUI()
    {
        GUI.matrix = uiCanvasFixer.GetGUIMatrix();

        Vector2 gridSize = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);
        Vector2 squareSize = new Vector2(gridSize.x / gridWidth, gridSize.y / gridHeight);

        //Draw HoriLines
        for (float y = startPos.y; y <= endPos.y; y += squareSize.y)
        {
            GUI.DrawTexture(new Rect(startPos.x, y, endPos.x - startPos.x, 4), line);
        }

        for (float x = startPos.x; x <= endPos.x; x += squareSize.x)
        {
            GUI.DrawTexture(new Rect(x, startPos.y, 4, endPos.y - startPos.y), line);
        }
    }
        
        /*

        GUIStyle style = new GUIStyle();
        style.fontSize = 30;

        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        GUI.Label(new Rect(0, 0, 300, 50), " Mouse Pos:" + mousePos, style);


        GUI.Label(new Rect(0, 50, 300, 50), " Matrix Mouse Pos:" + uiCanvasFixer.ScreenPosToCanvas(mousePos), style);

        Vector2Int oGrid = Vector2Int.zero;

        if (GetGridCoordinates(mousePos, ref oGrid, true))
        {
            GUI.Label(new Rect(0, 100, 300, 50), " Grid Cell " + oGrid, style);
        }

    }*/
}
