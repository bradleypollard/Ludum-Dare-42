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
}
