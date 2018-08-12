using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent( typeof( RectTransform ) )]
public class InfoPopUpOnly : Button
{
	//Constructor
	protected InfoPopUpOnly()
	{
	}

	//Variables
	private VisualGate m_visualGate;
	private VisualGridManager m_visualGridManager;
    private WireManager wireManager;

    private InfoPopup m_infoPopup;
	private bool m_isHighlighted;

	public override void OnPointerEnter( PointerEventData eventData )
	{
		base.OnPointerEnter( eventData );

		if ( m_infoPopup == null || m_visualGridManager == null || m_visualGate == null || wireManager == null)
		{
			m_infoPopup = FindObjectOfType<InfoPopup>();
			m_visualGridManager = FindObjectOfType<VisualGridManager>();
			m_visualGate = GetComponent<VisualGate>();
            wireManager = FindObjectOfType<WireManager>();
        }

        if (m_infoPopup != null)
        {
            if (m_infoPopup.SetText(m_visualGate.titleText, m_visualGate.descriptionText))
            {
                m_isHighlighted = true;
                StartCoroutine(OnHighlight());
            }
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (m_isHighlighted)
        {
            if (m_infoPopup != null)
            {
                m_infoPopup.PopText();
                m_isHighlighted = false;
            }
        }
    }

    private IEnumerator OnHighlight()
    {
        while (m_isHighlighted)
        {
            if (m_infoPopup != null)
            {
                Vector2 adjustedPos = m_visualGridManager.GetLocalPos(Input.mousePosition);
                m_infoPopup.SlideToY(adjustedPos.y);
            }
            yield return null;
        }
    }

}
