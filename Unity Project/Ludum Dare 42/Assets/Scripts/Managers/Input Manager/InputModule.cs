using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// V3.0 - InputModule
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - Acts as an Input Module to be used with Unity's Event Systems
/// </summary>
namespace RoboTools
{
    namespace Inputing
    {
        public class InputModule : PointerInputModule
        {
            public override void Process()
            {
                if (!eventSystem.isFocused)
                    return;

                SendUpdateEventToSelectedObject();

                if (input.mousePresent)
                    ProcessMouseEvent();
            }

            protected bool SendUpdateEventToSelectedObject()
            {
                if (eventSystem.currentSelectedGameObject == null)
                    return false;

                var data = GetBaseEventData();
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
                return data.used;
            }

            /// <summary>
            /// Process all mouse events.
            /// </summary>
            private void ProcessMouseEvent()
            {
                var mouseData = GetMousePointerEventData();

                var pressed = mouseData.AnyPressesThisFrame();
                var released = mouseData.AnyReleasesThisFrame();

                var leftButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData;

                if (!UseMouse(pressed, released, leftButtonData.buttonData))
                    return;

                // Process the first mouse button fully
                ProcessMousePress(leftButtonData);
                ProcessMove(leftButtonData.buttonData);
                ProcessDrag(leftButtonData.buttonData);

                // Now process right / middle clicks
                ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData);
                ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
                ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
                ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);

                if (!Mathf.Approximately(leftButtonData.buttonData.scrollDelta.sqrMagnitude, 0.0f))
                {
                    var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(leftButtonData.buttonData.pointerCurrentRaycast.gameObject);
                    ExecuteEvents.ExecuteHierarchy(scrollHandler, leftButtonData.buttonData, ExecuteEvents.scrollHandler);
                }
            }

            private static bool UseMouse(bool pressed, bool released, PointerEventData pointerData)
            {
                if (pressed || released || pointerData.IsPointerMoving() || pointerData.IsScrolling())
                    return true;

                return false;
            }

            /// <summary>
            /// Process the current mouse press.
            /// </summary>
            private void ProcessMousePress(MouseButtonEventData data)
            {
                var pointerEvent = data.buttonData;
                var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

                // PointerDown notification
                if (data.PressedThisFrame())
                {
                    pointerEvent.eligibleForClick = true;
                    pointerEvent.delta = Vector2.zero;
                    pointerEvent.dragging = false;
                    pointerEvent.useDragThreshold = true;
                    pointerEvent.pressPosition = pointerEvent.position;
                    pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                    DeselectIfSelectionChanged(currentOverGo, pointerEvent);

                    // search for the control that will receive the press
                    // if we can't find a press handler set the press
                    // handler to be what would receive a click.
                    var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

                    // didnt find a press handler... search for a click handler
                    if (newPressed == null)
                        newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                    // Debug.Log("Pressed: " + newPressed);

                    float time = Time.unscaledTime;

                    if (newPressed == pointerEvent.lastPress)
                    {
                        var diffTime = time - pointerEvent.clickTime;
                        if (diffTime < 0.3f)
                            ++pointerEvent.clickCount;
                        else
                            pointerEvent.clickCount = 1;

                        pointerEvent.clickTime = time;
                    }
                    else
                    {
                        pointerEvent.clickCount = 1;
                    }

                    pointerEvent.pointerPress = newPressed;
                    pointerEvent.rawPointerPress = currentOverGo;

                    pointerEvent.clickTime = time;

                    // Save the drag handler as well
                    pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                    if (pointerEvent.pointerDrag != null)
                        ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
                }

                // PointerUp notification
                if (data.ReleasedThisFrame())
                {
                    // Debug.Log("Executing pressup on: " + pointer.pointerPress);
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                    // Debug.Log("KeyCode: " + pointer.eventData.keyCode);

                    // see if we mouse up on the same element that we clicked on...
                    var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                    // PointerClick and Drop events
                    if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick)
                    {
                        ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                    }
                    else if (pointerEvent.pointerDrag != null)
                    {
                        ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
                    }

                    pointerEvent.eligibleForClick = false;
                    pointerEvent.pointerPress = null;
                    pointerEvent.rawPointerPress = null;

                    if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                        ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                    pointerEvent.dragging = false;
                    pointerEvent.pointerDrag = null;

                    // redo pointer enter / exit to refresh state
                    // so that if we moused over somethign that ignored it before
                    // due to having pressed on something else
                    // it now gets it.
                    if (currentOverGo != pointerEvent.pointerEnter)
                    {
                        HandlePointerExitAndEnter(pointerEvent, null);
                        HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                    }
                }
            }
        }
    }
}

