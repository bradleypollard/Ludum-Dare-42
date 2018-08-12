using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopup : MonoBehaviour
{
    public Text titleText, descriptionText;

    private bool isShowing;
    private bool isAnimating = false, currentShowing = false;
    private const float animTime = 0.5f, overSize = 0.2f, overSizePecent = 0.8f, fadePercent = 0.4f, overSizeTime = animTime * overSizePecent, fadeTime = animTime * fadePercent;

    private Vector2 desiredPos;
    private Queue<string> desiredTitle, desiredDescription;

    private WireManager wireManager;

    public bool locked;

    private void Start()
    {
        GetComponent<RectTransform>().localScale = isShowing ? Vector3.one : Vector3.zero;
        SetFade(isShowing ? 1.0f : 0.0f);
        desiredPos = GetComponent<RectTransform>().anchoredPosition;
        desiredTitle = new Queue<string>();
        desiredDescription = new Queue<string>();
    }

    private void Update()
    {
        //Update Text
        if (GetComponent<RectTransform>().localScale.x == 0.0f && desiredTitle.Count > 0)
        {
            titleText.text = desiredTitle.Peek();
            descriptionText.text = desiredDescription.Peek();
        }

        bool showWindow = desiredTitle.Count > 0;

        if (!isAnimating)
        {
            if (showWindow)
            {
                if (isShowing)
                {
                    if (desiredTitle.Peek() != titleText.text)
                    {
                        isAnimating = true;
                        StartCoroutine(Swap());
                    }
                }
                else
                {
                    isAnimating = true;
                    StartCoroutine(Show());
                }
            }
            else
            {
                if (isShowing)
                {
                    isAnimating = true;
                    StartCoroutine(Hide());
                }
            }
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GetComponent<RectTransform>().anchoredPosition, desiredPos, Time.deltaTime * 5.0f);
   
    }

    public void SlideToY(float _newPositionY)
    {
        desiredPos = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, _newPositionY);
    }

    public bool SetText(string _title, string _description)
    {
        if (!locked)
        {
            desiredTitle.Enqueue(_title);
            desiredDescription.Enqueue(_description);

            return true;
        }

        return false;
    }

    public void PopText()
    {
        desiredTitle.Dequeue();
        desiredDescription.Dequeue();
    }

    private IEnumerator Swap()
    {
        yield return ScaleToSize(0.0f);

        yield return null;
        yield return null;

        yield return ScaleToSize(1.0f);

        isAnimating = false;
        isShowing = true;
    }

    private IEnumerator Show()
    {
        yield return ScaleToSize(1.0f);
        isAnimating = false;
        isShowing = true;
    }

    private IEnumerator Hide()
    {
        yield return ScaleToSize(0.0f);
        isAnimating = false;
        isShowing = false;
    }

private IEnumerator ScaleToSize(float _size)
    {
        float startTime = Time.time, startScale = GetComponent<RectTransform>().localScale.x;

        while(Time.time - startTime < animTime)
        {
            if (_size > startScale)
            {
                if (Time.time - startTime < overSizeTime)
                {
                    GetComponent<RectTransform>().localScale = Vector3.one * Mathf.Lerp(startScale, _size + overSize, (Time.time - startTime) / overSizeTime);
                }
                else
                {
                    GetComponent<RectTransform>().localScale = Vector3.one * Mathf.Lerp(_size + overSize, _size, (Time.time - startTime - overSizeTime) / (animTime - overSizeTime));
                }

                if (Time.time - startTime < fadeTime)
                {
                    SetFade(Mathf.Lerp(0.0f, 1.0f, (Time.time - startTime) / fadeTime));
                }
            }
            else
            {
                float minusSize = animTime - overSizeTime;
                if (Time.time - startTime < minusSize)
                {
                    GetComponent<RectTransform>().localScale = Vector3.one * Mathf.Lerp(startScale, startScale + overSize, (Time.time - startTime) / minusSize);
                }
                else
                {
                    GetComponent<RectTransform>().localScale = Vector3.one * Mathf.Lerp(startScale + overSize, _size, (Time.time - startTime - minusSize) / (animTime - minusSize));
                }

                float minusFade = animTime - fadeTime;
                if (Time.time - startTime > minusFade)
                {
                    SetFade(Mathf.Lerp(1.0f, 0.0f, (Time.time - startTime - minusFade) / (animTime - minusFade)));
                }
            }

            yield return null;
        }

        //Snap Scale at End
        GetComponent<RectTransform>().localScale = Vector3.one * _size;
        SetFade(_size > startScale ? 1.0f : 0.0f);
    }

    private void SetFade(float _alpha)
    {
        Color imageColour = GetComponent<Image>().color;
        imageColour.a = _alpha;
        GetComponent<Image>().color = imageColour;

        foreach(Text text in GetComponentsInChildren<Text>())
        {
            Color textColour = text.color;
            textColour.a = _alpha;
            text.color = textColour;
        }
    }
}
