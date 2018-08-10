using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using RoboTools.Inputing;

/// <summary>
/// V1.1 - CustomInputField
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - Allows Unity's Inputfield Component to be incorporated into the UI Connection Manager
/// </summary>
namespace RoboTools
{
    namespace UI
    {
        public class CustomInputField : UIConnection
        {
            bool isLocked;
            int lockedPlayer = -1;

            private GameObject uiKeyboardPrefab, uiKeyboardSpawned;
            private InputType lastInputType = InputType.None;

            //Components
            RectTransform myRectTransform;
            InputField myInputField;

            public override void Setup()
            {
                uiKeyboardPrefab = Resources.Load<GameObject>("RoboTools/UI/UIKeyboard");
                myRectTransform = GetComponent<RectTransform>();
                myInputField = GetComponent<InputField>();
            }

            public override void Update()
            {
                base.Update();

                if (lockedPlayer != -1 && InputManager.controllers[lockedPlayer].inputType != lastInputType)
                {
                    lastInputType = InputManager.controllers[lockedPlayer].inputType;

                    if (lastInputType == InputType.Keyboard && uiKeyboardSpawned != null)
                    {
                        Destroy(uiKeyboardSpawned);
                    }
                    else if (lastInputType != InputType.Keyboard && uiKeyboardSpawned == null)
                    {
                        SetupKeyboard(lockedPlayer);
                    }

                    eventSystem.SetSelectedGameObject(gameObject);
                    InputManager.controllers[lockedPlayer].localLocked = true;
                }
            }

            public override void OnSubmit(int _playerID)
            {
                if (!isLocked)
                {
                    isLocked = true;
                    eventSystem.SetSelectedGameObject(gameObject);
                    InputManager.controllers[_playerID].localLocked = true;
                    lockedPlayer = _playerID;
                    lastInputType = InputManager.controllers[_playerID].inputType;

                    SetupKeyboard(_playerID);
                }
                else
                {
                    Unlock();
                }
            }

            public override void OnClicked(int _playerID)
            {
                if (!isLocked)
                {
                    isLocked = true;
                    eventSystem.SetSelectedGameObject(gameObject);
                    InputManager.controllers[_playerID].localLocked = true;
                    lockedPlayer = _playerID;
                    lastInputType = InputManager.controllers[_playerID].inputType;
                }
            }

            public override void OnCancelled(int _playerID)
            {
                if (isLocked)
                {
                    Unlock();
                }
            }

            public override void OnSelected()
            {
                if (GetComponent<Image>() != null)
                {
                    GetComponent<Image>().color = selectedColour;
                }

                if (isLocked)
                {
                    myRectTransform.localScale = Vector3.Lerp(myRectTransform.localScale, Vector3.one * 1.1f, Time.unscaledDeltaTime * 8f);
                }
                else
                {
                    myRectTransform.localScale = Vector3.Lerp(myRectTransform.localScale, Vector3.one * 1.025f, Time.unscaledDeltaTime * 8f);
                }
            }

            public override void OnUnSelected()
            {
                if (GetComponent<Image>() != null)
                {
                    GetComponent<Image>().color = normalColour;
                }
                myRectTransform.localScale = Vector3.Lerp(myRectTransform.localScale, Vector3.one, Time.unscaledDeltaTime * 8f);

                Unlock();
            }

            private void Unlock()
            {
                if (isLocked)
                {
                    isLocked = false;
                    eventSystem.SetSelectedGameObject(null);
                    InputManager.controllers[lockedPlayer].localLocked = false;
                    lockedPlayer = -1;

                    if(uiKeyboardSpawned != null)
                    {
                        Destroy(uiKeyboardSpawned);
                        uiKeyboardSpawned = null;
                    }
                }
            }

            private void SetupKeyboard(int _playerID)
            {
                if (!uiKeyboardSpawned && InputManager.controllers[_playerID].inputType != InputType.Keyboard)
                {
                    uiKeyboardSpawned = Instantiate(uiKeyboardPrefab, transform.parent);

                    //SetupKeyboard
                    UIKeyboardController controller = uiKeyboardSpawned.GetComponent<UIKeyboardController>();
                    controller.Setup(myInputField);
                }
            }

            public override void OnDestroy()
            {
                base.OnDestroy();

                myInputField.DeactivateInputField();
            }
        }
    }
}

