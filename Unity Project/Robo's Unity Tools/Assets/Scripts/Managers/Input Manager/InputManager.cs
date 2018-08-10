using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoboTools.Saving;
using System.IO;

/// <summary>
/// V3.3 - InputManager
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - Handles all Input and Management of Controllers
/// </summary>
namespace RoboTools
{
    namespace Inputing
    {
        public enum GameInputType
        {
            OnePlayerOverride, //Using a new controller will override Player One
            PartyMode, //Each Controller is assigned to a Player and must be locked in / out
        }

        //Input States for Party Mode
        public enum InputState
        {
            Locked,
            LockedShowing,
            MeetAmount,
            Any,
            AnyMinusKeyboard
        }

        public enum MouseMode
        {
            Hidden,
            Shown,
            ShownForKeyboard,
            ShownWhenMouseMoved
        }

        public class InputManager : ISavingManager
        {
            public GameInputType localGameInputType;
            public static GameInputType gameInputType { get; private set; }
         
            public InputState localInput;
            public static InputState inputState { get; private set; }

            public MouseMode localMouseMode;
            public static MouseMode mouseMode { get; private set; }

            public bool forcePlayerOne = false;

            public const int maxPlayers = 4, maxJoysticks = 11;

            public static int targetAmount { get; private set; }

            /// <summary>
            /// Returns the controllers in the order they were connected
            /// </summary>
            public static Dictionary<int, InputDevice> controllers;
            public static List<ControlLayout> allAvailableConfigs;

            public static List<List<ControlLayout>> configsPerType;

            public static bool mouseLock;

            // Use this for initialization
            void Awake()
            {
                //Create the list of controllers for the Game
                controllers = new Dictionary<int, InputDevice>();
                SetAllControllerLayouts(new List<ControlLayout>());
                
                inputState = localInput;
                gameInputType = localGameInputType;
                mouseMode = localMouseMode;

                if(gameInputType == GameInputType.OnePlayerOverride || forcePlayerOne)
                {
                    AddController(new KeyboardDevice());
                }
            }

            // Adds a New Controller to the First Blank Space in the Dictionary
            public static void AddController(InputDevice _newDevice)
            {
                int possibleKey = controllers.Count;
                for(int i = 0; i < controllers.Count; i++)
                {
                    if(!controllers.ContainsKey(i))
                    {
                        possibleKey = i;
                        break;
                    }
                }

                controllers.Add(possibleKey, _newDevice);
            }

            // Removes a Controller
            public static void RemoveController(int _deviceID)
            {
                controllers.Remove(_deviceID);
            }

            //Sort the Dictionary
            public static void SortControllers()
            {
                Dictionary<int, InputDevice> replacementDictionary = new Dictionary<int, InputDevice>();
                int count = 0;

                foreach(KeyValuePair<int, InputDevice> pair in controllers)
                {
                    replacementDictionary.Add(count++, pair.Value);
                }

                controllers = replacementDictionary;
            }

            void Update()
            {
                //Update Local Vals for Inspector
                localInput = inputState;

                //Do Mouse Behaviour
                switch (mouseMode)
                {
                    case MouseMode.Hidden:
                        {
                            Cursor.visible = false;
                            break;
                        }
                    case MouseMode.Shown:
                        {
                            Cursor.visible = true;
                            break;
                        }
                    case MouseMode.ShownForKeyboard:
                        {
                            bool showMouse = false;

                            foreach(InputDevice inputDevice in controllers.Values)
                            {
                                if(inputDevice.inputType == InputType.Keyboard)
                                {
                                    showMouse = true;
                                    break;
                                }
                            }

                            Cursor.visible = showMouse;
                            break;
                        }
                    case MouseMode.ShownWhenMouseMoved:
                        {
                            bool recivedKey = false;
                            for (int i = 1; i < 300; i++)
                            {
                                if (Input.GetKey((KeyCode)i))
                                {
                                    recivedKey = true;
                                    break;
                                }
                            }

                            //Check Xbox
                            if(!recivedKey)
                            {
                                for (int i = 1; i < maxJoysticks; i++)
                                {
                                    if (RecieveInput(i))
                                    {
                                        recivedKey = true;
                                        break;
                                    }
                                }
                            }

                            if (Cursor.visible && recivedKey)
                            {
                                Cursor.visible = false;
                            }

                            if(!Cursor.visible && (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2) || Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f))
                            {
                                Cursor.visible = true;
                            }

                            break;
                        }
                }

                /*Hide Mouse if it's not used
                if (Cursor.lockState == CursorLockMode.Locked || ((Input.anyKey || (controllers.Count > 0 && (controllers[0].GetRawInput("MenuHorizontal") != 0 || controllers[0].GetRawInput("MenuVertical") != 0)))
                    && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2)))
                {
                    Cursor.visible = false;
                }

                if (Cursor.lockState != CursorLockMode.Locked && (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2) || Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f))
                    Cursor.visible = true;
                */

                if (mouseLock && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
                {
                    mouseLock = false;
                }

                //Check for new inputs
                if (inputState != InputState.Locked && inputState != InputState.LockedShowing)
                {
                    if (controllers.Count < maxPlayers)
                    {
                        //Get a list of Joysticks in Use
                        List<int> inputsInUse = new List<int>();
                        foreach (InputDevice controller in controllers.Values)
                            inputsInUse.Add(controller.joystickID);

                        //Get Keyboard Inputs (If not already in Use)
                        if (!inputsInUse.Contains(0))
                        {
                            bool recivedKey = false;

                            for (int i = 1; i < 300; i++)
                            {
                                if (Input.GetKey((KeyCode)i))
                                {
                                    recivedKey = true;
                                    break;
                                }
                            }

                            if (recivedKey || Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f)
                            {
                                switch (gameInputType)
                                {
                                    case GameInputType.PartyMode:
                                        {
                                            if (inputState != InputState.AnyMinusKeyboard)
                                            {
                                                AddController(new KeyboardDevice());
                                            }
                                            break;
                                        }
                                    case GameInputType.OnePlayerOverride:
                                        {
                                            if (controllers[0].inputType != InputType.Keyboard)
                                            {
                                                controllers[0] = new KeyboardDevice(controllers[0]);
                                            }
                                            break;
                                        }
                                }
                            }
                        }

                        //Get Joystick Inputs
                        for (int i = 1; i <= maxJoysticks; i++)
                        {
                            //If Joystick is not in use
                            if (!inputsInUse.Contains(i))
                            {
                                //If we get an input
                                if (RecieveInput(i))
                                {
                                    if (inputState == InputState.Locked || inputState == InputState.LockedShowing)
                                    {
                                        //Do Locked GUI Flash
                                    }
                                    else
                                    {
                                        //Detect Type of Input Device (Or don't)                      

                                        switch (gameInputType)
                                        {
                                            case GameInputType.PartyMode:
                                                {
                                                    if (inputState != InputState.AnyMinusKeyboard)
                                                    {
                                                        //Add this Joystick to the Controllers
                                                        AddController(new XBox360Device(i));
                                                    }
                                                    break;
                                                }
                                            case GameInputType.OnePlayerOverride:
                                                {
                                                    if (controllers[0].inputType != InputType.Xbox360)
                                                    {
                                                        controllers[0] = new XBox360Device(i, controllers[0]);
                                                    }
                                                    break;
                                                }
                                        }                                     
                                    }
                                }
                            }
                        }
                    }
                }

                //Controller Removing 
                if (gameInputType == GameInputType.PartyMode)
                {
                    List<int> toKill = new List<int>();
                    foreach (KeyValuePair<int, InputDevice> controller in controllers)
                    {
                        //Do Controller Removing
                        if (controller.Value.GetButton("Leave"))
                        {
                            if (inputState != InputState.Locked && inputState != InputState.LockedShowing)
                            {
                                toKill.Add(controller.Key);                            
                            }
                            else
                            {
                                //Do Locked GUI Flash
                            }
                        }
                    }

                    foreach(int keyToKill in toKill)
                    {
                        RemoveController(keyToKill);
                    }
                }

                //Controller Unlocking and Toggling
                //bool firstController = true;
                foreach (InputDevice controller in controllers.Values)
                {
                    //Do InputLock unlocking
                    foreach (KeyValuePair<string, string> record in new Dictionary<string, string>(controller.inputLocks))
                    {
                        if (record.Value != "")
                        {
                            if (controller.GetRawInput(record.Value, LockOverride.OverrideLocalLock) == 0f)
                            {
                                controller.inputLocks[record.Key] = "";
                                controller.inputTimer = 0f;
                            }
                        }
                    }
                }
            }

#if UNITY_EDITOR
            void OnGUI()
            {
                GUI.depth = -100;

                //If we're not in the locked state, render active controllers
                if (controllers.Count > 0)
                {
                    //Foreach Controller
                    foreach (KeyValuePair<int, InputDevice> controller in controllers)
                    {
                        int i = controller.Key;
                        InputDevice device = controller.Value;
                        GUI.Box(new Rect(10 + (i * 110), 10, 100, 25), i.ToString() + ": " + device.inputType.ToString());
                    }
                }
            }
#endif

            //Checks if there is any Joystick Input
            public bool RecieveInput(int joyStick)
            {
                int startVal = 330 + (joyStick * 20);

                for (int i = startVal; i < startVal + 20; i++)
                {
                    if (Input.GetKey((KeyCode)i))
                        return true;
                }

                string[] axisToCheck = { "X", "Y", "3", "4", "5", "6", "7" };

                foreach (string axis in axisToCheck)
                {
                    if (Input.GetAxis("Joystick" + joyStick + "Axis" + axis) != 0)
                    {
                        return true;
                    }
                }

                return false;
            }

            //Checks for a click
            public static bool GetClick(int _mouseButton)
            {
                if (!mouseLock && Input.GetMouseButtonDown(_mouseButton))
                {
                    mouseLock = true;
                    return true;
                }

                return false;
            }

            //Checks for a click
            public static bool GetClickHold(int _mouseButton)
            {
                if (Input.GetMouseButton(_mouseButton))
                {
                    return true;
                }

                return false;
            }

            //Remove all controllers minus the first
            public static void RemoveAllButOneController()
            {
                SortControllers();

                while (controllers.Count > 1)
                {
                    RemoveController(controllers.Count - 1);
                }
            }

            public static void SetAllControllerLayouts(List<ControlLayout> newList)
            {
                allAvailableConfigs = newList;

                //Create a second list where layouts are grouped by type
                configsPerType = new List<List<ControlLayout>>();

                int maxAmount = Enum.GetValues(typeof(InputType)).Length;

                for (int i = 0; i < maxAmount; i++)
                    configsPerType.Add(new List<ControlLayout>());

                //Add Defaults
                configsPerType[0].Add(KeyboardDevice.myDefault);
                configsPerType[1].Add(XBox360Device.myDefault);

                foreach (ControlLayout controlLayout in allAvailableConfigs)
                    configsPerType[(int)controlLayout.inputType].Add(controlLayout);
            }

            public static void SetInputState(InputState _newState)
            {
                //Set the amount we need to meet
                if (_newState == InputState.MeetAmount)
                    targetAmount = controllers.Count;

                inputState = _newState;
            }

            public static void SetMouseMode(MouseMode _mouseMode)
            {
                mouseMode = _mouseMode;
            }

            public static string GetXboxInput(ref bool usesAxes)
            {
                //Array of all Axes to check
                string[] possibleControls = new string[] { "AxisX", "AxisY", "Axis3", "Axis4", "Axis5", "Axis6", "Axis7" };

                for (int i = 1; i <= InputManager.maxJoysticks; i++)
                {
                    for (int j = 0; j < possibleControls.Length; j++)
                    {
                        float value = Input.GetAxisRaw("Joystick" + i + possibleControls[j]);
                        if (value > 0)
                        {
                            usesAxes = true;
                            return possibleControls[j] + "+";
                        }
                        else if (value < 0)
                        {
                            usesAxes = true;
                            return possibleControls[j] + "-";
                        }
                    }
                }

                //Check for Button
                for (int j = 0; j < 20; j++)
                {
                    if (Input.GetKey((KeyCode)(330 + j)))
                    {
                        usesAxes = false;
                        string returnVal = ((KeyCode)(330 + j)).ToString();

                        //Remove Joystick X from string
                        returnVal = returnVal.Remove(0, returnVal.IndexOf("Button"));

                        //Make Button Lowercase
                        returnVal = returnVal.Replace("Button", "button");

                        //Add a space after button 
                        returnVal = returnVal.Insert(returnVal.IndexOf("Button") + 7, " ");

                        return returnVal;

                    }
                }

                return "";
            }

            public static int GetMousePlayer()
            {
                foreach (KeyValuePair<int, InputDevice> pair in controllers)
                {
                    if(pair.Value.inputType == InputType.Keyboard)
                    {
                        return pair.Key;
                    }
                }

                return -1;
            }

            // ----------------------------- Saving Methods --------------------------------------

            public override void DoSave(BinaryWriter _stream)
            {
                string toSave = "";

                foreach (ControlLayout cl in allAvailableConfigs)
                {
                    if (toSave != "")
                        toSave += ";";

                    toSave += cl.ToString();
                }

                _stream.Write(toSave);
            }

            public override void DoLoad(int _version, BinaryReader _stream)
            {
                allAvailableConfigs = new List<ControlLayout>();

                //Get string from Player Prefs
                string toLoad = _stream.ReadString();

                if (toLoad != "")
                {
                    //Split the string into each Controller Layout
                    string[] inputs = toLoad.Split(';');

                    foreach (string inputData in inputs)
                    {
                        //Parse the controller layout from the string. If an exception is thrown ignore that string
                        // try
                        //{
                        ControlLayout cl = ControlLayout.Parse(inputData);
                        allAvailableConfigs.Add(cl);
                        /*}
                        catch (Exception err)
                        {
                            Debug.LogError(err.Message);
                        }*/
                    }
                }

                SetAllControllerLayouts(allAvailableConfigs);
            }
        }
    }
}
