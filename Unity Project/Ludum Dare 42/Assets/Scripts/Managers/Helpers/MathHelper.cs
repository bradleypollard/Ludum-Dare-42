using UnityEngine;
using System.Collections;

/// <summary>
/// V1.0 - MathHelper
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - How's some useful Math Functions
/// </summary>
namespace RoboTools
{
    namespace Helpers
    {
        public class MathHelper
        {
            public static int NumClamp(int value, int min, int max)
            {
                int diff = (max - min);

                while (value >= max)
                    value -= diff;

                while (value < min)
                    value += diff;

                return value;
            }

            public static float NumClamp(float value, float min, float max)
            {
                float diff = (max - min);

                while (value >= max)
                    value -= diff;

                while (value < min)
                    value += diff;

                return value;
            }

            /// <summary>
            /// Returns the angle between two vectors in a clockwise motion
            /// </summary>
            /// <param name="toDirection"></param>
            /// <param name="fromDirection"></param>
            /// <returns></returns>
            public static float Angle(Vector3 toDirection, Vector3 fromDirection)
            {
                float toReturn = Vector3.Angle(toDirection, fromDirection);

                Vector3 rightDir = Quaternion.AngleAxis(90, Vector3.up) * toDirection;
                if (Vector3.Angle(fromDirection, rightDir) > 90)
                    toReturn = -toReturn;

                return toReturn;
            }

            public static int Sign(float value)
            {
                if (value == 0f)
                    return 0;
                else
                    return (int)Mathf.Sign(value);
            }

            public static IEnumerator FadeImage(UnityEngine.UI.Image _image, float startAlpha, float endAlpha, float _travelTime)
            {
                //Hide Title
                float startTime = Time.time;
                while (Time.time - startTime < _travelTime)
                {
                    _image.color = new Color(1f, 1f, 1f, Mathf.Lerp(startAlpha, endAlpha, (Time.time - startTime) / _travelTime));
                    yield return null;
                }

                _image.color = new Color(1f, 1f, 1f, endAlpha);
            }

            public static IEnumerator ScaleTransform(Transform transform, float startSize, float endSize, float _travelTime, Vector3 _default)
            {
                //Hide Title
                float startTime = Time.time;
                while (Time.time - startTime < _travelTime)
                {
                    transform.localScale = Mathf.Lerp(startSize, endSize, (Time.time - startTime) / _travelTime) * _default;
                    yield return null;
                }

                transform.localScale = endSize * _default;
            }

            public static IEnumerator ScaleTransform(Transform transform, float startSize, float endSize, float _travelTime)
            {
                yield return ScaleTransform(transform, startSize, endSize, _travelTime, Vector3.one);
            }
        }
    }
}