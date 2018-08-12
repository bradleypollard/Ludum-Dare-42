using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent( typeof( RectTransform ) )]
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
	private WireManager m_wireManager;

	// Methods
	public virtual new void OnPointerDown( PointerEventData _eventData )
	{
		//Check for null Transform
		if ( _eventData.button == PointerEventData.InputButton.Left )
		{
			if ( m_rectTransform == null || m_canvas == null || ( conformToGrid && m_visualGridManager == null ) || m_gameplayManager == null || m_wireManager == null )
			{
				m_rectTransform = GetComponent<RectTransform>();
				m_canvas = FindObjectOfType<Canvas>();
				m_visualGridManager = FindObjectOfType<VisualGridManager>();
				m_gameplayManager = FindObjectOfType<GameplayManager>();
				m_wireManager = FindObjectOfType<WireManager>();
			}

			if ( m_rectTransform != null && m_canvas != null && ( !conformToGrid || m_visualGridManager != null ) && m_gameplayManager != null && m_wireManager != null )
			{
				if ( m_wireManager.IsInWireEditMode() )
				{
					DoStateTransition( SelectionState.Pressed, false );
					m_isBeingDragged = true;
					StartCoroutine( OnDrag() );
				}
				else
				{
					DoStateTransition( SelectionState.Pressed, false );
					m_isBeingDragged = true;
					StartCoroutine( OnDrag() );
				}

			}
		}
	}

	public virtual new void OnPointerUp( PointerEventData _eventData )
	{
		if ( _eventData.button == PointerEventData.InputButton.Left )
		{
			DoStateTransition( currentSelectionState, false );
			m_isBeingDragged = false;

			Vector2Int oGrid = Vector2Int.zero;
			if ( m_visualGridManager.GetGridCoordinates( m_rectTransform.anchoredPosition, ref oGrid, false ) )
			{
				CellCoordinates cell = new CellCoordinates( (uint)oGrid.x, (uint)oGrid.y );
				GameObject copy = Instantiate( gameObject, transform.parent );

				VisualGate visualGate = GetComponent<VisualGate>();
				if ( visualGate != null )
				{
					if ( visualGate.gateType != GateType.IncrementDecrement )
					{
						m_gameplayManager.AddGate( visualGate.gateType, cell, visualGate.objectOrientation );
					}
					else
					{
						m_gameplayManager.AddIncrementDecrementGate( cell, visualGate.objectOrientation, visualGate.value );
					}

					copy.GetComponent<RectTransform>().anchoredPosition = visualGate.GetSpawnLocation();
				}
				VisualWire visualWire = GetComponent<VisualWire>();
				if ( visualWire != null )
				{
					m_gameplayManager.AddWire( visualWire.wireType, cell, visualWire.objectOrientation );
					copy.GetComponent<RectTransform>().anchoredPosition = visualWire.GetSpawnLocation();
				}
			}
			else
			{
				VisualGate visualGate = GetComponent<VisualGate>();
				if ( visualGate != null )
				{
					GetComponent<RectTransform>().anchoredPosition = visualGate.GetSpawnLocation();
				}
				VisualWire visualWire = GetComponent<VisualWire>();
				if ( visualWire != null )
				{
					GetComponent<RectTransform>().anchoredPosition = visualWire.GetSpawnLocation();
				}
			}
		}
		else if ( _eventData.button == PointerEventData.InputButton.Right )
		{
			if ( m_isBeingDragged )
			{
				VisualGate visualGate = GetComponent<VisualGate>();
				if ( visualGate != null )
				{
					visualGate.Rotate( true );
				}
				VisualWire visualWire = GetComponent<VisualWire>();
				if ( visualWire != null )
				{
					visualWire.Rotate( true );
				}
			}
		}
	}

	private IEnumerator OnDrag()
	{
		while ( m_isBeingDragged )
		{
			//Drag Logic
			Vector2 mousePosition = m_visualGridManager.GetSnapPoint( new Vector2( Input.mousePosition.x, Input.mousePosition.y ) );
			m_rectTransform.anchoredPosition = mousePosition;
			yield return null;
		}
	}

	private IEnumerator OnDragWire()
	{
		while ( m_isBeingDragged )
		{
			//Drag Logic
			Vector2 mousePosition = m_visualGridManager.GetSnapPoint( new Vector2( Input.mousePosition.x, Input.mousePosition.y ) );

			Vector2Int oGrid = Vector2Int.zero;
			if ( m_visualGridManager.GetGridCoordinates( mousePosition, ref oGrid, false ) )
			{
				CellCoordinates cell = new CellCoordinates( (uint)oGrid.x, (uint)oGrid.y );
			}
			else
			{

			}
			yield return null;
		}
	}
}
