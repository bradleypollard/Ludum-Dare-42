using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// V1.0 - Actor
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - Absract class used to control Gameobjects
/// </summary>
namespace RoboTools
{
    namespace Dialogue
    {
        public abstract class Actor : MonoBehaviour
        {
            public abstract void StartAnimation(string _animationName, bool _isLooping, params object[] _params);
            public abstract void StopAnimation();
        }
    }
}