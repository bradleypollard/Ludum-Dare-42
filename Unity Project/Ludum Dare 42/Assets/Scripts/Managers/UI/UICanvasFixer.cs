using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UICanvasFixer : MonoBehaviour
{
    //Components
    CanvasScaler canvasScaler;

	// Use this for initialization
	void Start ()
    {
        canvasScaler = GetComponent<CanvasScaler>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        float screenWidth = Screen.width, screenHeight = Screen.height;

        float canvasScalerMatch = 0.0f;
        float widthRatio = screenWidth / canvasScaler.referenceResolution.x;

        if(canvasScaler.referenceResolution.y * widthRatio >= screenHeight)
        {
            canvasScalerMatch = 1.0f;
        }

        canvasScaler.matchWidthOrHeight = canvasScalerMatch;

        //Debug.Log("My Pos:" + Input.mousePosition);
    }

    public Matrix4x4 GetGUIMatrix()
    {
        float width = canvasScaler.referenceResolution.x;
        float height = canvasScaler.referenceResolution.y;

        float wScale = Screen.width / width;
        float hScale = Screen.height / height;

        float wStart = (Screen.width - (width * hScale)) / 2;
        float hStart = (Screen.height - (height * wScale)) / 2;

        if (Screen.width > (width * hScale))
        {
            //screenEdges = new Rect(-wStart / hScale, 0f, Screen.width / hScale, Screen.height / hScale);

            //float newWidth = (Screen.height / 1080f) * Screen.width;
            //guiEdges = new Rect(wStart, 0f, newWidth, Screen.height);
            //widthSmaller = false;

            return Matrix4x4.TRS(new Vector3(wStart, 0, 0), Quaternion.identity, new Vector3(hScale, hScale, 1));
        }
        else
        {
            //screenEdges = new Rect(0f, -hStart / wScale, Screen.width / wScale, Screen.height / wScale);

            //float newHeight = (Screen.width / 1920f) * Screen.height;
           // guiEdges = new Rect(0f, hStart, Screen.width, newHeight);
           // widthSmaller = true;

            return Matrix4x4.TRS(new Vector3(0, hStart, 0), Quaternion.identity, new Vector3(wScale, wScale, 1));
        }
    }

    public Matrix4x4 GetScreenToCanvasScaleMatrix()
    {
        float screenWidth = Screen.width, screenHeight = Screen.height;

        float widthRatio = screenWidth / canvasScaler.referenceResolution.x;
        float heightRatio = screenHeight / canvasScaler.referenceResolution.y;

        if (canvasScaler.referenceResolution.y * widthRatio >= screenHeight)
        {
            //Height of screen is 1080
            screenWidth = canvasScaler.referenceResolution.x * heightRatio;

            float wStart = (Screen.width - screenWidth) / 2;
            return Matrix4x4.Scale(new Vector3(heightRatio, heightRatio, 1.0f));

        }
        else
        {
            //Width of screen is 1920
            screenHeight = canvasScaler.referenceResolution.y * heightRatio;

            float hStart = (Screen.height - screenWidth) / 2;
            return Matrix4x4.Scale(new Vector3(widthRatio, widthRatio, 1.0f));
        }
    }

    public Vector2 GetStartPos()
    {
        float screenWidth = Screen.width, screenHeight = Screen.height;

        float widthRatio = screenWidth / canvasScaler.referenceResolution.x;
        float heightRatio = screenHeight / canvasScaler.referenceResolution.y;

        if (canvasScaler.referenceResolution.y * widthRatio >= screenHeight)
        {
            //Height of screen is 1080
            screenWidth = canvasScaler.referenceResolution.x * heightRatio;

            float wStart = (Screen.width - screenWidth) / 2;
            float hScale = Screen.height / canvasScaler.referenceResolution.y;

            return new Vector2(wStart, 0.0f) / hScale;

        }
        else
        {
            //Width of screen is 1920
            screenHeight = canvasScaler.referenceResolution.y * widthRatio;

            float hStart = (Screen.height - screenHeight) / 2;
            float wScale = Screen.width / canvasScaler.referenceResolution.x;

            return new Vector2(0.0f, hStart) / wScale;
        }
    }

    public Vector2 ScreenPosToCanvas(Vector2 _screenPos)
    {
        Vector2 newPos = GetScreenToCanvasScaleMatrix().inverse * _screenPos;
        newPos -= GetStartPos();

        //Vector2 newMousePos = uiCanvasFixer.GetScreenToCanvasScaleMatrix().inverse * mousePos;
        //newMousePos -= uiCanvasFixer.GetStartPos();

        return newPos;
    }
}
