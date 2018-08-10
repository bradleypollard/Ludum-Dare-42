using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using RoboTools.Inputing;

/// <summary>
/// V1.1 - UIConnectionManager
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - Manages UI Connections
/// </summary>
namespace RoboTools
{
    namespace UI
    {
        public class UIConnectionManager : MonoBehaviour
        {
            public enum UserControlMode { OneCursor, CursorPerPlayer}
            public UserControlMode userControlMode { get; private set; }
            public UserControlMode localUserControlMode;

            public Dictionary<int, UIConnection> currentUIConnection;
            public UIConnection[] debugArray;

            private List<UIConnection> allConnectors;
            private EventSystem eventSystem;

            public int playerCanUse = -1;//-1 = All, 0 = Player 1, 1 = Player 2 .etc

            // Use this for initialization
            void Awake()
            {
                allConnectors = new List<UIConnection>();
                SetUserControlMode(localUserControlMode);
            }

            public void AddConnector(UIConnection _uiConnection)
            {
                if (allConnectors != null)
                {
                    allConnectors.Add(_uiConnection);
                }
            }

            public void RemoveConnector(UIConnection _uiConnection)
            {
                if (allConnectors != null)
                {
                    allConnectors.Remove(_uiConnection);
                }
            }

            public void SetUserControlMode(UserControlMode _userControlMode)
            {
                userControlMode = _userControlMode;

                currentUIConnection = new Dictionary<int, UIConnection>();
            }

            private static bool IsNull(UIConnection s)
            {
                return s == null;
            }

            // Update is called once per frame
            void Update()
            {
                //Ensure we have enough current UI Connections
                if(userControlMode == UserControlMode.CursorPerPlayer)
                {
                    //Controller Adding
                    foreach(KeyValuePair<int, InputDevice> controller in InputManager.controllers)
                    {
                        if(!currentUIConnection.ContainsKey(controller.Key))
                        {
                            currentUIConnection.Add(controller.Key, null);
                        }
                    }

                    //Controller Removing
                    List<int> indexToRemove = new List<int>();
                    foreach (KeyValuePair<int, UIConnection> connection in currentUIConnection)
                    {
                        if (!InputManager.controllers.ContainsKey(connection.Key))
                        {
                            indexToRemove.Add(connection.Key);
                        }
                    }

                    foreach(int index in indexToRemove)
                    {
                        currentUIConnection.Remove(index);
                    }
                }
                else
                {
                    if(InputManager.controllers.Count == 0 && currentUIConnection.Count > 0)
                    {
                        currentUIConnection.Remove(-1);
                    }
                    else if (InputManager.controllers.Count > 0 && currentUIConnection.Count == 0)
                    {
                        currentUIConnection.Add(-1, null);
                    }
                }

                //Clear all Null Button
                allConnectors.RemoveAll(IsNull);

                //Mouse Input and Selected/Unselected Behaviour
                bool mouseOnAnything = false;
                foreach (UIConnection uiConnector in allConnectors.ToArray())
                {
                    RectTransform rectTransform = uiConnector.GetComponent<RectTransform>();
                    Button button = uiConnector.GetComponent<Button>();
                    Toggle toggle = uiConnector.GetComponent<Toggle>();
                    InputField inputField = uiConnector.GetComponent<InputField>();

                    //Do Open and Close if Button
                    if (uiConnector.isActiveAndEnabled && rectTransform != null && (button != null || inputField != null || toggle != null))
                    {
                        //Mouse Checks
                        if (Cursor.visible && (playerCanUse == -1 || (playerCanUse) == InputManager.GetMousePlayer()))
                        {
                            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, null))
                            {
                                currentUIConnection[playerCanUse] = uiConnector;
                                mouseOnAnything = true;
                            }
                        }

                        bool selected = false;
                        
                        foreach (KeyValuePair<int, UIConnection> connection in new Dictionary<int, UIConnection>(currentUIConnection))
                        {
                            //Set as default for each input if Priority
                            if (connection.Value == null && uiConnector.priority)
                            {
                                currentUIConnection[connection.Key] = uiConnector;
                            }

                            //Do On Selected if Player has selected this
                            if (connection.Value == uiConnector)
                            {
                                selected = true;
                                break;
                            }
                        }     
                        
                        if(selected)
                        {
                            uiConnector.OnSelected();
                        }
                        else
                        {
                            uiConnector.OnUnSelected();
                        }
                    }
                }

                //Behaviour for each Controller
                foreach (KeyValuePair<int, UIConnection> connection in new Dictionary<int, UIConnection>(currentUIConnection))
                {
                    //Unselect Option if it becomes unavailable
                    if (connection.Value != null && !connection.Value.isActiveAndEnabled)
                    {
                        currentUIConnection[connection.Key] = null;
                    }
                }   

                if (eventSystem == null)
                {
                    eventSystem = FindObjectOfType<EventSystem>();
                }
                else if (eventSystem.isActiveAndEnabled)
                {
                    //Do Input
                    foreach (KeyValuePair<int, InputDevice> controller in InputManager.controllers)
                    {
                        int playerID = playerCanUse == -1 ? controller.Key : playerCanUse;
                        int uiConnectionIndex = userControlMode == UserControlMode.CursorPerPlayer ? playerID : -1;

                        if (playerID == controller.Key)
                        {
                            InputDevice player = controller.Value;
                            UIConnection playerConnection = currentUIConnection[uiConnectionIndex];

                            if (playerConnection)
                            {                               
                                if (player.GetRawButtonWithLock("Submit", LockOverride.OverrideLocalLock))
                                {
                                    if (playerConnection.GetComponent<Button>() != null)
                                    {
                                        playerConnection.GetComponent<Button>().onClick.Invoke();
                                    }

                                    if (playerConnection.GetComponent<Toggle>() != null)
                                    {
                                        playerConnection.GetComponent<Toggle>().onValueChanged.Invoke(!playerConnection.GetComponent<Toggle>().isOn);
                                    }

                                    playerConnection.OnSubmit(playerID);
                                }

                                if (player.GetRawButtonWithLock("Cancel", LockOverride.OverrideLocalLock))
                                {
                                    playerConnection.OnCancelled(playerID);
                                }

                                if (mouseOnAnything && player.inputType == InputType.Keyboard && InputManager.GetClick(0))
                                {
                                    playerConnection.OnClicked(playerID);
                                }

                                if (!mouseOnAnything)
                                {
                                    int hori = player.GetRawIntInputWithDelay("MenuHorizontal", 0.25f, Time.unscaledDeltaTime);
                                    int vert = player.GetRawIntInputWithDelay("MenuVertical", 0.25f, Time.unscaledDeltaTime);

                                    //Navigation
                                    if (hori > 0 && playerConnection.OnRight != null)
                                    {
                                        currentUIConnection[uiConnectionIndex] = playerConnection.OnRight;
                                    }

                                    if (hori < 0 && playerConnection.OnLeft != null)
                                    {
                                        currentUIConnection[uiConnectionIndex] = playerConnection.OnLeft;
                                    }

                                    if (vert < 0 && playerConnection.OnUp != null)
                                    {
                                        currentUIConnection[uiConnectionIndex] = playerConnection.OnUp;
                                    }

                                    if (vert > 0 && playerConnection.OnDown != null)
                                    {
                                        currentUIConnection[uiConnectionIndex] = playerConnection.OnDown;
                                    }
                                }
                            }
                        }
                    }
                }

                debugArray = new UIConnection[currentUIConnection.Count];
                int count = 0;

                foreach (KeyValuePair<int, UIConnection> controller in currentUIConnection)
                {
                    debugArray[count] = currentUIConnection[controller.Key];
                    count++;
                }

            } // Update
        }
    }
}
