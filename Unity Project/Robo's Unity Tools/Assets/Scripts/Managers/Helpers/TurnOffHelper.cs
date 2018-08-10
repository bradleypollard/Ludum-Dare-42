using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// V1.0 - TurnOffHelper
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - Turn's off Gameobjects when the Scene is loaded
/// </summary>
namespace RoboTools
{
    namespace Helpers
    {
        public class TurnOffHelper : MonoBehaviour
        {
            public List<GameObject> turnOn, turnOff;

            // Use this for initialization
            void Awake()
            {
                foreach (GameObject gameObject in turnOn)
                {
                    gameObject.SetActive(true);
                }

                foreach (GameObject gameObject in turnOff)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}