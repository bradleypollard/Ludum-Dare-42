using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// V1.0 - UIConnection
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - Allows a Unity UI Component to be incorporated into the UI Connection Manager
/// </summary>
namespace RoboTools
{
    namespace UI
    {
        public abstract class UIConnection : MonoBehaviour
        {
            protected UIConnectionManager myManager;
            public UIConnection OnUp, OnDown, OnLeft, OnRight;
            public bool priority;

            public abstract void OnSelected();
            public abstract void OnUnSelected();

            public abstract void    OnSubmit(int _playerID);
            public virtual void     OnClicked(int _playerID) { OnSubmit(_playerID); }

            public virtual void     OnCancelled(int _playerID) { }

            protected static EventSystem eventSystem;

            protected static Color normalColour = new Color(0.7f, 0.7f, 0.7f, 0.588f);
            protected static Color hoverColour = new Color(1f, 1f, 1f, 0.588f);
            protected static Color selectedColour = new Color(1f, 1f, 1f, 0.588f);

            public void Start()
            {
                myManager = FindObjectOfType<UIConnectionManager>();
                myManager.AddConnector(this);

                Setup();
            }

            public virtual void Setup() { }

            public virtual void Update()
            {
                if (eventSystem == null)
                {
                    eventSystem = FindObjectOfType<EventSystem>();
                }
            }

            public virtual void OnDestroy()
            {
                if (myManager)
                {
                    myManager.RemoveConnector(this);
                }

                if(eventSystem.currentSelectedGameObject == gameObject)
                {
                    eventSystem.SetSelectedGameObject(null);
                }
            }
        }
    }
}
