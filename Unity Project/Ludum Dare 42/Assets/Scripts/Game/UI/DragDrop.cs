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
    private GameplayManager m_gameplayManager;

    // Methods
    public virtual new void OnPointerDown(PointerEventData _eventData)
    {
        //Check for null Transform
        if (m_rectTransform == null || m_canvas == null || (conformToGrid && m_visualGridManager == null) || m_gameplayManager == null)
        {
            m_rectTransform = GetComponent<RectTransform>();
            m_canvas = FindObjectOfType<Canvas>();
            m_visualGridManager = FindObjectOfType<VisualGridManager>();
            m_gameplayManager = FindObjectOfType<GameplayManager>();
        }

        if (m_rectTransform != null && m_canvas != null && (!conformToGrid || m_visualGridManager != null) && m_gameplayManager != null)
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

        Vector2Int oGrid = Vector2Int.zero;
        if (m_visualGridManager.GetGridCoordinates(m_rectTransform.anchoredPosition, ref oGrid, false))
        {
            CellCoordinates cell = new CellCoordinates((uint)oGrid.x, (uint)oGrid.y);

            VisualGate visualGate = GetComponent<VisualGate>();
            if (visualGate != null)
            {
                m_gameplayManager.AddGate(visualGate.gateType, cell, ObjectOrientation.Or0);
            }
            VisualWire visualWire = GetComponent<VisualWire>();
            if (visualWire != null)
            {
                m_gameplayManager.AddWire(visualWire.wireType, cell, ObjectOrientation.Or0);
            }
        }     
    }

    private IEnumerator OnDrag()
    {
        while(m_isBeingDragged)
        {
            //Drag Logic
            Vector2 mousePosition = m_visualGridManager.GetSnapPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            m_rectTransform.anchoredPosition = mousePosition;
            yield return null;
        }
    }
}
