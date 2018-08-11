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

    public Vector2 GetSnapPoint(Vector2 _position)
    {
        Vector2 actualPos = uiCanvasFixer.ScreenPosToCanvas(_position);

        if (actualPos.x > startPos.x && actualPos.y > startPos.y && actualPos.x < endPos.x && actualPos.y < endPos.y)
        {
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

    public bool GetGridCoordinates(Vector2 _position, ref Vector2Int o_position, bool _convertToCanvas)
    {
        Vector2 actualPos = _convertToCanvas ? uiCanvasFixer.ScreenPosToCanvas(_position) : _position;

        if (actualPos.x > startPos.x && actualPos.y > startPos.y && actualPos.x < endPos.x && actualPos.y < endPos.y)
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

    }
}
