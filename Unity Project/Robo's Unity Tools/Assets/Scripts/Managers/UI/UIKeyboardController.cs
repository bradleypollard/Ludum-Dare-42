using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIKeyboardController : MonoBehaviour
{
    public const int idealWidth = 400, idealHeight = 200;

    public int keyboardWidth, keyboardHeight;
    public int playerCanUse = -1;//-1 = All, 0 = Player 1, 1 = Player 2 .etc

    private InputField connectedInputField;
    private RectTransform connectedInputFieldRect;
    private List<GameObject> keys;

    private readonly string[] QWERTY = new string[] { "QWERTYUIOP", "ASDFGHJKL", "ZXCVBNM" };

    //Keys
    private GameObject keyPrefab;

    //Components
    RectTransform rectTransform;
    RectTransform canvasTransform;

    void Start ()
    {
        //Get Components
        rectTransform = GetComponent<RectTransform>();

        //Setup Keyboard
        ForceKeyboardSize(idealWidth, idealHeight);

        //Canvas Transform
        Transform parent = transform;
        while(parent.parent != null)
        {
            parent = transform.parent;
        }
        canvasTransform = parent.GetComponent<RectTransform>();

        rectTransform.localScale = Vector3.zero;

        keyPrefab = Resources.Load<GameObject>("RoboTools/UI/UIKey");
    }

    //Setup the Keyboard
    public void Setup(InputField _connectedInputField, int _width, int _height)
    {
        connectedInputField = _connectedInputField;
        connectedInputFieldRect = connectedInputField.GetComponent<RectTransform>();

        ForceKeyboardSize(_width, _height);
    }
    public void Setup(InputField _connectedInputField) { Setup(_connectedInputField, idealWidth, idealHeight); }

    // Advance
    private void Update()
    {
        if(rectTransform.sizeDelta.x != keyboardWidth || rectTransform.sizeDelta.y != keyboardHeight)
        {
            rectTransform.sizeDelta = new Vector2(keyboardWidth, keyboardHeight);
        }

        if (connectedInputField != null)
        {
            Vector2 anchoredPos = connectedInputFieldRect.anchoredPosition - new Vector2(0, (connectedInputFieldRect.sizeDelta.y / 2f) + (keyboardHeight / 2f) + 5);

            //Clamp anchoredPos inside of Screen
            float halfCanvasWidth = canvasTransform.sizeDelta.x /2f, halfCanvasHeight = canvasTransform.sizeDelta.y / 2f;
            float halfKeyboardWidth = keyboardWidth / 2f, halfKeyboardHeight = keyboardHeight / 2f;

            anchoredPos.x = Mathf.Clamp(anchoredPos.x, -halfCanvasWidth + halfKeyboardWidth + 5, halfCanvasWidth - halfKeyboardWidth - 5);
            anchoredPos.y = Mathf.Clamp(anchoredPos.y, -halfCanvasHeight + halfKeyboardHeight + 5, halfCanvasHeight - halfKeyboardHeight - 5);

            rectTransform.anchoredPosition = anchoredPos;
            rectTransform.localScale = Vector3.one;
        }
        else
        {
            rectTransform.localScale = Vector3.zero;
        }
    }

    // Helpers	 
    public void ForceKeyboardSize(int _width, int _height)
    {
        keyboardWidth = _width;
        keyboardHeight = _height;

        //Clear Old Keys
        if(keys != null)
        {
            foreach(GameObject key in keys)
            {
                Destroy(key);
            }
        }

        //Spawn new Keys
        keys = new List<GameObject>();
    }
}
