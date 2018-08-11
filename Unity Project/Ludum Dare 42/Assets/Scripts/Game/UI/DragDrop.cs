using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class DragDrop : Selectable, IPointerDownHandler, IPointerUpHandler
{
    //Constructor
    protected DragDrop()
    {
    }

    //Variables
    private bool m_isBeingDragged;
    private Vector2 m_mosuePos;
    private RectTransform m_rectTransform;
    private Canvas m_canvas;

    public bool conformToGrid;
    private VisualGridManager m_visualGridManager; 

    // Methods
    public virtual new void OnPointerDown(PointerEventData _eventData)
    {
        //Check for null Transform
        if (m_rectTransform == null || m_canvas == null || (conformToGrid && m_visualGridManager == null))
        {
            m_rectTransform = GetComponent<RectTransform>();
            m_canvas = FindObjectOfType<Canvas>();
            m_visualGridManager = FindObjectOfType<VisualGridManager>();
        }

        if (m_rectTransform != null && m_canvas != null && (!conformToGrid || m_visualGridManager != null))
        {
            DoStateTransition(SelectionState.Pressed, false);
            m_isBeingDragged = true;
            StartCoroutine(OnDrag());
        }
    }

    public virtual new void OnPointerUp(PointerEventData _eventData)
    {
        DoStateTransition(currentSelectionState, false);
        m_isBeingDragged = false;
    }

    private IEnumerator OnDrag()
    {
        while(m_isBeingDragged)
        {
            //Drag Logic
            Vector2 screenHalf = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            Vector2 mousePosition = m_visualGridManager.GetSnapPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            m_rectTransform.anchoredPosition = (mousePosition - screenHalf) / m_canvas.scaleFactor;
            yield return null;
        }
    }
}
