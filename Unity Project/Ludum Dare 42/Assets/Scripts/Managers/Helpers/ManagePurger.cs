using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// V1.0 - ManagePurger
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - Ensures that only one gameobject with a ManagePurger survives
/// </summary>
namespace RoboTools
{
    namespace Helpers
    {
        public class ManagePurger : MonoBehaviour
        {
            private static ManagePurger alpha = null;

            // Use this for initialization
            void Awake()
            {
                if (alpha == null)
                {
                    alpha = this;
                    DontDestroyOnLoad(gameObject);
                }

                if (this != alpha)
                {
                    Debug.Log("I'm less than perfect! Must kill self!");

                    DestroyImmediate(gameObject);
                }
            }
        }
    }
}
