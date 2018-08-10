using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RoboTools.Helpers;

/// <summary>
/// V1.0 - InputDevice
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - Handles all Controller / Keyboard Input
/// </summary>
namespace RoboTools
{
    namespace Inputing
    {
        public enum InputType { Keyboard, Xbox360, None }
        public enum LockOverride
        {
            None,
            OverrideLocalLock, // Overrides an Input Devices Local Lock
        }

        public abstract class InputDevice
        {
            //Lock off all inputs
            public static bool locked = false;
            //Lock off this specific input
            public bool localLocked = false;

            //What type of controller is this
            public abstract InputType inputType { get; }

            //What is the ID for this Joystick. -1 = NULL, 0 = Keyboard, 1> = Joystick
            public int joystickID = -1;

            //Provide a Default Layout for the Controller
            public abstract ControlLayout defaultLayout { get; }

            //The layout we are currently using
            public ControlLayout currentLayout { get; private set; }
            public void SetLayout(ControlLayout _currentLayout)
            {
                currentLayout = _currentLayout;
            }

            public Dictionary<string, string> inputLocks = new Dictionary<string, string>() { {"Default", "" } };
            public float inputTimer = 0f;

            public float inputBirthTime { get; private set; }

            //Constructor
            public InputDevice(int _joystickID)
            {
                //Set inital layout to defeault
                currentLayout = defaultLayout;

                joystickID = _joystickID;
                inputLocks["Default"] = "Submit";

                inputBirthTime = Time.realtimeSinceStartup;
            }

            enum WantedInput { Either, PlusOnly, MinusOnly, Inverse }

            //Get Inputs

            //Input Behaviours Behaviour
            private float ActualGetInput(string _input, bool _getRaw, LockOverride _overrideLock = LockOverride.None) { return ActualGetAllInput(_input, _getRaw, currentLayout, _overrideLock); }

            private float ActualGetInputWithLock(string _input, bool _getRaw, LockOverride _overrideLock = LockOverride.None, string inputLock = "Default")
            {
                if(!inputLocks.ContainsKey(inputLock))
                {
                    inputLocks.Add(inputLock, "");
                }

                if (inputLocks[inputLock] == "")
                {
                    float val = ActualGetInput(_input, _getRaw, _overrideLock);
                    if (val != 0)
                    {
                        inputLocks[inputLock] = _input;
                        return val;
                    }
                }

                return 0;
            }

            private float ActualGetInputWithDelay(string _input, bool _getRaw, float _delay, float _deltaTime, float _cutOff = 0.5f, LockOverride _overrideLock = LockOverride.None, string inputLock = "Default")
            {
                if (!inputLocks.ContainsKey(inputLock))
                {
                    inputLocks.Add(inputLock, "");
                }

                float val = GetInput(_input, _overrideLock);

                if (inputTimer <= 0f && (inputLocks[inputLock] == _input || inputLocks[inputLock] == ""))
                {
                    if (Mathf.Abs(val) >= _cutOff)
                    {
                        inputLocks[inputLock] = _input;
                        inputTimer = _delay;
                        return val;
                    }
                }

                if (inputLocks[inputLock] == _input && val != 0f)
                {
                    inputTimer -= _deltaTime;
                }

                return 0;
            }

            //Accessible Methods - Input ---------------------------------------------------------------------------------------------------------------------------
            public float GetInput(string _input, LockOverride _overrideLock = LockOverride.None) { return ActualGetInput(_input, false, _overrideLock); }
            public float GetRawInput(string _input, LockOverride _overrideLock = LockOverride.None) { return ActualGetInput(_input, true, _overrideLock); }

            public int GetIntInput(string _input, LockOverride _overrideLock = LockOverride.None) { return MathHelper.Sign(ActualGetInput(_input, false, _overrideLock)); }
            public int GetRawIntInput(string _input, LockOverride _overrideLock = LockOverride.None) { return MathHelper.Sign(ActualGetInput(_input, true, _overrideLock)); }

            public bool GetButton(string _input, LockOverride _overrideLock = LockOverride.None) { return ActualGetInput(_input, false, _overrideLock) != 0; }
            public bool GetRawButton(string _input, LockOverride _overrideLock = LockOverride.None) { return ActualGetInput(_input, true, _overrideLock) != 0; }

            //Accessible Methods - Input With Lock ------------------------------------------------------------------------------------------------------------------
            public float GetInputWithLock(string _input, LockOverride _overrideLock = LockOverride.None, string inputLock = "Default") { return ActualGetInputWithLock(_input, false, _overrideLock, inputLock); }
            public float GetRawInputWithLock(string _input, LockOverride _overrideLock = LockOverride.None, string inputLock = "Default") { return ActualGetInputWithLock(_input, true, _overrideLock, inputLock); }

            public int GetIntInputWithLock(string _input, LockOverride _overrideLock = LockOverride.None, string inputLock = "Default") { return MathHelper.Sign(ActualGetInputWithLock(_input, false, _overrideLock, inputLock)); }
            public int GetRawIntInputWithLock(string _input, LockOverride _overrideLock = LockOverride.None, string inputLock = "Default") { return MathHelper.Sign(ActualGetInputWithLock(_input, true, _overrideLock, inputLock)); }

            public bool GetButtonWithLock(string _input, LockOverride _overrideLock = LockOverride.None, string inputLock = "Default") { return ActualGetInputWithLock(_input, false, _overrideLock, inputLock) != 0; }
            public bool GetRawButtonWithLock(string _input, LockOverride _overrideLock = LockOverride.None, string inputLock = "Default") { return ActualGetInputWithLock(_input, true, _overrideLock, inputLock) != 0; }

            //Accessible Methods - Input With Delay ------------------------------------------------------------------------------------------------------------------
            public float GetInputWithDelay(string _input, float _delay, float _deltaTime, float _cutOff = 0.5f, LockOverride _overrideLock = LockOverride.None, string inputLock = "Default") { return ActualGetInputWithDelay(_input, false, _delay, _deltaTime, _cutOff, _overrideLock, inputLock); }
            public float GetRawInputWithDelay(string _input, float _delay, float _deltaTime, float _cutOff = 0.5f, LockOverride _overrideLock = LockOverride.None, string inputLock = "Default") { return ActualGetInputWithDelay(_input, true, _delay, _deltaTime, _cutOff, _overrideLock, inputLock); }

            public int GetIntInputWithDelay(string _input, float _delay, float _deltaTime, float _cutOff = 0.5f, LockOverride _overrideLock = LockOverride.None, string inputLock = "Default") { return MathHelper.Sign(ActualGetInputWithDelay(_input, false, _delay, _deltaTime, _cutOff, _overrideLock, inputLock)); }
            public int GetRawIntInputWithDelay(string _input, float _delay, float _deltaTime, float _cutOff = 0.5f, LockOverride _overrideLock = LockOverride.None, string inputLock = "Default") { return MathHelper.Sign(ActualGetInputWithDelay(_input, true, _delay, _deltaTime, _cutOff, _overrideLock, inputLock)); }

            public bool GetButtonWithDelay(string _input, float _delay, float _deltaTime, float _cutOff = 0.5f, LockOverride _overrideLock = LockOverride.None, string inputLock = "Default") { return ActualGetInputWithDelay(_input, false, _delay, _deltaTime, _cutOff, _overrideLock, inputLock) != 0; }
            public bool GetRawButtonWithDelay(string _input, float _delay, float _deltaTime, float _cutOff = 0.5f, LockOverride _overrideLock = LockOverride.None, string inputLock = "Default") { return ActualGetInputWithDelay(_input, true, _delay, _deltaTime, _cutOff, _overrideLock, inputLock) != 0; }

            // -------------------------------------------------------------------------------------------------------------------------------------------------------
            private float ActualGetAllInput(string _input, bool getRaw, ControlLayout controlLayout, LockOverride _overrideLock = LockOverride.None)
            {
                float returnVal = 0f;

                if ((!localLocked || _overrideLock == LockOverride.OverrideLocalLock) && !locked && controlLayout != null)
                {
                    if (controlLayout.controls.ContainsKey(_input))
                    {
                        foreach (ControlName control in controlLayout.controls[_input])
                        {
                            string buttonName = "";

                            //Add Joystick name to button if not Keyboard
                            if (inputType != InputType.Keyboard)
                            {
                                if (!control.isAxis)
                                    buttonName = "joystick " + joystickID + " ";
                                else
                                    buttonName = "Joystick" + joystickID;
                            }

                            //Add Input
                            buttonName += control.inputName;

                            //Check for + / - or * symbol 
                            //Side Note: Using +/- on an axis will only allow values which are positive or negative. 
                            //Using on a button will return either a positive or negative value
                            //Using * will inverse an axis
                            WantedInput wantedInput = WantedInput.Either;

                            if (buttonName[buttonName.Length - 1] == '-')
                                wantedInput = WantedInput.MinusOnly;
                            else if (buttonName[buttonName.Length - 1] == '+')
                                wantedInput = WantedInput.PlusOnly;
                            else if (buttonName[buttonName.Length - 1] == '*')
                                wantedInput = WantedInput.Inverse;

                            //Remove the + or - from the input
                            if (wantedInput != WantedInput.Either)
                                buttonName = buttonName.Remove(buttonName.Length - 1, 1);

                            //Check if input is an axis or a button
                            if (control.isAxis)
                            {
                                //Get the actual input
                                float tempValue = 0f;
                                if (getRaw)
                                    tempValue = Input.GetAxisRaw(buttonName);
                                else
                                    tempValue = Input.GetAxis(buttonName);

                                if (tempValue != 0f)
                                {
                                    //Assign the temp value if it is valid
                                    switch (wantedInput)
                                    {
                                        case WantedInput.Either: returnVal = tempValue; break;
                                        case WantedInput.MinusOnly: if (tempValue <= 0f) returnVal = tempValue; break;
                                        case WantedInput.PlusOnly: if (tempValue >= 0f) returnVal = tempValue; break;
                                        case WantedInput.Inverse: returnVal = -tempValue; break;
                                    }
                                }
                            }
                            else
                            {
                                //Get the actual input if button is pressed
                                returnVal = Input.GetKey(buttonName) ? (wantedInput == WantedInput.MinusOnly ? -1 : 1) : 0;
                            }

                            //Break out of inputs if input found
                            if (returnVal != 0f)
                                break;
                        }
                    }
                    else
                    {
                        if (controlLayout == defaultLayout)
                            Debug.LogError(_input + " is not a registered input!");
                        else if (!InputIgnoreList.ignoreList.Contains(_input))
                            return ActualGetAllInput(_input, getRaw, defaultLayout);
                    }
                }

                return returnVal;
            }
        }

        public class ControlLayout
        {
            //Dictionary containing all input names and what button/axis are used for them
            public Dictionary<string, List<ControlName>> controls;
            public InputType inputType { get; set; }
            public string layoutName;

            public ControlLayout(InputType _inputType, string _name, Dictionary<string, List<ControlName>> _controls)
            {
                inputType = _inputType;
                controls = _controls;
                layoutName = _name;
            }

            //Returns current state of Control Layout for Saving
            public override string ToString()
            {
                string toReturn = ((int)inputType).ToString() + "|" + layoutName;

                foreach (KeyValuePair<string, List<ControlName>> pair in controls)
                {
                    if (pair.Value.Count > 0)
                    {
                        toReturn += "|";

                        toReturn += pair.Key;

                        foreach (ControlName cn in pair.Value)
                        {
                            toReturn += ":";
                            toReturn += cn.inputName;
                        }
                    }
                }

                return toReturn;
            }

            //Parse current string into Control Layout
            public static ControlLayout Parse(string _data)
            {
                //Split the data
                List<string> allPairs = _data.Split('|').ToList();

                //Make the return value
                ControlLayout toReturn = new ControlLayout((InputType)int.Parse(allPairs[0]), allPairs[1], new Dictionary<string, List<ControlName>>());

                //Remove input type and name from array
                allPairs.RemoveRange(0, 2);

                foreach (string pair in allPairs)
                {
                    //Split each input string into another list
                    string[] array = pair.Split(':');

                    //If we already have this input throw an exception
                    if (toReturn.controls.ContainsKey(array[0]))
                        throw new Exception("Input already exists!");

                    //Create a new list of control names
                    List<ControlName> inputs = new List<ControlName>();
                    for (int i = 1; i < array.Length; i++)
                        inputs.Add(new ControlName(array[i], (array[i].Length > 4f && array[i].Substring(0, 4) == "Axis") ? true : false));

                    toReturn.controls.Add(array[0], inputs);
                }

                return toReturn;
            }

            public void ChangeInput(string _input, string _button, bool _usesAxis, int position)
            {
                //If the input dosen't exist create it
                if (!controls.ContainsKey(_input))
                    controls.Add(_input, new List<ControlName>());

                //Make sure we have an input to replace
                while (controls[_input].Count < position + 1)
                    controls[_input].Add(new ControlName("", false));

                controls[_input][position] = new ControlName(_button, _usesAxis);
            }

            public void ClearInput(string _input, int position)
            {
                //If the input dosen't exist create it
                if (controls.ContainsKey(_input) && controls[_input].Count > position)
                {
                    controls[_input].RemoveAt(position);
                }
            }
        }

        public class ControlName
        {
            public string inputName { get; private set; }
            public bool isAxis { get; private set; }

            public ControlName(string _inputName, bool _isAxis)
            {
                inputName = _inputName;
                isAxis = _isAxis;
            }
        }
    }
}