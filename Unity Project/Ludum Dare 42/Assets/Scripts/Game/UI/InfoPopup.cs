using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopup : MonoBehaviour
{
    public Text titleText, descriptionText;

    public bool isShowing = false;
    private bool isAnimating = false, currentShowing = false;
    private const float animTime = 0.5f, overSize = 0.2f, overSizePecent = 0.8f, fadePercent = 0.4f, overSizeTime = animTime * overSizePecent, fadeTime = animTime * fadePercent;

    private Vector2 desiredPos;
    private string desiredTitle, desiredDescription;

    private void Start()
    {
        GetComponent<RectTransform>().localScale = isShowing ? Vector3.one : Vector3.zero;
        SetFade(isShowing ? 1.0f : 0.0f);
        desiredPos = GetComponent<RectTransform>().anchoredPosition;
    }

    private void Update()
    {
        //Update Text
        if (GetComponent<RectTransform>().localScale.x == 0.0f)
        {
            titleText.text = desiredTitle;
            descriptionText.text = desiredDescription;
        }

        if (isShowing != currentShowing && !isAnimating)
        {
            isAnimating = true;
            currentShowing = isShowing;

            if (isShowing)
            {
                StartCoroutine(ScaleToSize(1.0f));
            }
            else
            {
                StartCoroutine(ScaleToSize(0.0f));
            }
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GetComponent<RectTransform>().anchoredPosition, desiredPos, Time.deltaTime * 5.0f);
   
    }

    public void SlideToY(float _newPositionY)
    {
        desiredPos = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, _newPositionY);
    }

    public void SetText(string _title, string _description)
    {
        desiredTitle = _title;
        desiredDescription = _description;
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

        isAnimating = false;
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
