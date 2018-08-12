using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent( typeof( RectTransform ) )]
public class DragDropGate : Selectable, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
	//Constructor
	protected DragDropGate()
	{
	}

	//Variables
	private bool m_isBeingDragged;
	private Vector2 m_mosuePos;
	private RectTransform m_rectTransform;
	private Canvas m_canvas;

    private VisualGate m_visualGate;
	private VisualGridManager m_visualGridManager;
	private GameplayManager m_gameplayManager;
	private WireManager m_wireManager;

	private bool m_isPlaced;
	private CellCoordinates m_cellCoordinates;

    private InfoPopup m_infoPopup;
    private bool m_isHighlighted;

    // Methods
    public virtual new void OnPointerDown(PointerEventData _eventData)
    {
        //Check for null Transform
        if (_eventData.button == PointerEventData.InputButton.Left)
        {
            if (m_rectTransform == null || m_canvas == null || m_visualGate == null ||  m_visualGridManager == null || m_gameplayManager == null || m_wireManager == null || m_infoPopup == null)
            {
                m_rectTransform = GetComponent<RectTransform>();
                m_visualGate = GetComponent<VisualGate>();
                m_canvas = FindObjectOfType<Canvas>();
                m_visualGridManager = FindObjectOfType<VisualGridManager>();
                m_gameplayManager = FindObjectOfType<GameplayManager>();
				m_wireManager = FindObjectOfType<WireManager>();
                m_infoPopup = FindObjectOfType<InfoPopup>();

            }
            if (m_rectTransform != null && m_canvas != null && m_visualGate != null && m_visualGridManager != null && m_gameplayManager != null && m_wireManager != null )
            {
				if ( !m_wireManager.IsInWireEditMode() )
				{ 
					DoStateTransition(SelectionState.Pressed, false);
					m_isBeingDragged = true;
                    m_rectTransform.SetParent(m_gameplayManager.gridParent);

                    StartCoroutine(OnDrag());
                }
            }
        }
    }


    public virtual new void OnPointerUp(PointerEventData _eventData)
    {
        if (_eventData.button == PointerEventData.InputButton.Left)
        {
			if ( !m_wireManager.IsInWireEditMode() )
			{
				DoStateTransition(currentSelectionState, false);
				m_isBeingDragged = false;

                if (m_infoPopup != null && m_isHighlighted)
                {
                    m_isHighlighted = false;
                    m_infoPopup.PopText();
                }

                //If we were on a space clear it
                if (m_isPlaced)
                {
                    m_gameplayManager.ClearCell(m_cellCoordinates);
                }

                Vector2Int oGrid = Vector2Int.zero;

                if (m_visualGridManager.GetGridCoordinates(m_rectTransform.anchoredPosition, ref oGrid, false)
                    && m_visualGridManager.IsGridObjectValid(m_rectTransform.anchoredPosition, m_visualGate, false))
                {
                    CellCoordinates cell = new CellCoordinates((uint)oGrid.x, (uint)oGrid.y);
                    m_cellCoordinates = cell;
                    m_isPlaced = true;

                    if (m_visualGate != null)
                    {
                        GameObject copy = Instantiate(gameObject, m_gameplayManager.scrollViewParent);
                        copy.GetComponent<VisualBase>().ResetBase();

                        VisualGate visualGate = GetComponent<VisualGate>();
                        if (visualGate != null)
                        {
                            if (visualGate.gateType != GateType.IncrementDecrement)
                            {
                                m_gameplayManager.AddGate(visualGate.gateType, cell, visualGate.objectOrientation, gameObject);
                            }
                            else
                            {
                                m_gameplayManager.AddIncrementDecrementGate(cell, visualGate.objectOrientation, visualGate.value, gameObject);
                            }

                            copy.GetComponent<RectTransform>().anchoredPosition = visualGate.GetSpawnLocation();
                        }
                    }
                }
                else
                {
                    if (!m_isPlaced)
                    {
                        if (m_visualGate != null)
                        {
                            GetComponent<RectTransform>().SetParent(m_gameplayManager.scrollViewParent);
                            GetComponent<RectTransform>().anchoredPosition = m_visualGate.GetSpawnLocation();
                            GetComponent<VisualBase>().ResetBase();
                        }
                    }
                    else
                    {
                        //If we have already been placed. Kill yourself
                        Destroy(gameObject);
                    }
                }
            }
        }
    }


	private IEnumerator OnDrag()
	{
		while ( m_isBeingDragged )
		{
			//Drag Logic

			Vector2 mousePosition = m_visualGridManager.GetSnapPoint( new Vector2( Input.mousePosition.x, Input.mousePosition.y ), m_visualGate);
			m_rectTransform.anchoredPosition = mousePosition;

            if (Input.GetMouseButtonUp(1))
            {
                VisualGate visualGate = GetComponent<VisualGate>();
                if (visualGate != null)
                {
                    visualGate.Rotate(true);
                }
            }

            yield return null;
		}
	}

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if(m_infoPopup == null || m_visualGridManager == null || m_visualGate == null)
        {
            m_infoPopup = FindObjectOfType <InfoPopup>();
            m_visualGridManager = FindObjectOfType<VisualGridManager>();
            m_visualGate = GetComponent<VisualGate>();
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

        if (!m_isBeingDragged && m_isHighlighted)
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
        while(m_isHighlighted)
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
