using UnityEngine;
using System;
using System.Collections;
using static Tweeny.Function;
using System.Diagnostics;

namespace Tweeny
{
    //TweenyAnimations

        //PhyscsAnimation ?

    [Serializable]
    public static class Animation
    {
        public delegate IEnumerator AnimationHandler(FunctionHandler function, float duration, GameObject gameObject, params object[] param);

        public static class Transformation
        {

            public static IEnumerator Play(AnimationName animation, FunctionName function, float duration, GameObject gameObject, params object[] param)
            {
                IEnumerator enumerator = Animations[(int)animation](
                    Functions[(int)function], duration, gameObject, param);
                Tweeny.Instance.StartCoroutine(enumerator);
                return enumerator;
            }

            public static void Stop(IEnumerator coroutine)
            {
                Tweeny.Instance.StopCoroutine(coroutine);
            }

            public enum AnimationName
            {
                Move,
                Rotate,
                Scale
            }

            //Keep this two similar

            public static AnimationHandler[] Animations = new AnimationHandler[]
            {
                Move,
                Rotate,
                Scale
            };

            public static IEnumerator Move(FunctionHandler function, float duration, GameObject obj, params object[] param)
            {
                Vector3 point = GetParam<Vector3>(true,param);
                Space space = GetParam<Space>(param);
                Action action = GetParam<Action>(param);
                Timer time = GetParam<Timer>(param);
                bool cicle = GetParam<bool>(param);
                Vector3 startPosition = GetStartPosition(obj, space);
                do
                {
                    float timer = 0;
                    if (space == Space.World)
                        if (action == Action.Straight)
                            while (timer <= duration)
                            {
                                obj.transform.position = Vector3.Lerp(startPosition, point, function(timer / duration));
                                yield return timer += TimeScale(time);
                            }
                        else
                            while (timer <= duration)
                            {
                                obj.transform.position += Vector3.Lerp(Vector3.zero, point, function(timer / duration));
                                yield return timer += TimeScale(time);
                            }
                    else
                        if (action == Action.Straight)                       
                            while (timer <= duration)
                            {
                                obj.transform.localPosition = Vector3.Lerp(startPosition, point, function(timer / duration));
                                yield return timer += TimeScale(time);
                            }                       
                        else                       
                            while (timer <= duration)
                            {
                                obj.transform.localPosition += Vector3.Lerp(Vector3.zero, point, function(timer / duration));
                                yield return timer += TimeScale(time);
                            }                                         
                    yield return true;
                } while (cicle);
            }
            public static IEnumerator Rotate(FunctionHandler function, float duration, GameObject obj, params object[] param)
            {
                Quaternion point = Quaternion.Euler(GetParam<Vector3>(true, param));
                Space space = GetParam<Space>(param);
                Action action = GetParam<Action>(param);
                Timer time = GetParam<Timer>(param);
                bool cicle = GetParam<bool>(param);
                Quaternion startRotation = GetStartRotation(obj, space);
                do
                {
                    float timer = 0;
                    if (space == Space.World)
                        if (action == Action.Straight)
                            while (timer <= duration)
                            {
                                obj.transform.rotation = Quaternion.Lerp(startRotation, point, function(timer / duration));
                                yield return timer += TimeScale(time);
                            }
                        else
                            while (timer <= duration)
                            {
                                obj.transform.rotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), point, function(timer / duration));
                                yield return timer += TimeScale(time);
                            }
                    else
                        if (action == Action.Straight)
                        while (timer <= duration)
                        {
                            obj.transform.localRotation = Quaternion.Lerp(startRotation, point, function(timer / duration));
                            yield return timer += TimeScale(time);
                        }
                    else
                        while (timer <= duration)
                        {
                            obj.transform.localRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), point, function(timer / duration));
                            yield return timer += TimeScale(time);
                        }
                    yield return true;
                } while (cicle);
            }

            public static IEnumerator Scale(FunctionHandler function, float duration, GameObject obj, params object[] param)
            {
                Vector3 point = GetParam<Vector3>(true, param);
                Action action = GetParam<Action>(param);
                Timer time = GetParam<Timer>(param);
                bool cicle = GetParam<bool>(param);
                Vector3 startScale = obj.transform.localScale;
                do
                {
                    float timer = 0;
                    if (action == Action.Straight)
                        while (timer <= duration)
                        {
                            obj.transform.localScale = Vector3.Lerp(startScale, point, function(timer / duration));
                            yield return timer += TimeScale(time);
                        }
                    else
                        while (timer <= duration)
                        {
                            obj.transform.localScale += Vector3.Lerp(Vector3.zero, point, function(timer / duration));
                            yield return timer += TimeScale(time);
                        }
                    yield return true;
                } while (cicle);
            }
        }

        public static class RigidBody
        {

        }

        public static class Rendering
        {

        }

        public static class Camera
        {

        }

        public static class UI
        {
            public static IEnumerator Blinking(FunctionHandler function, float duration, GameObject obj, params object[] param)
            {
                UnityEngine.UI.Text text = obj.GetComponent<UnityEngine.UI.Text>();
                Color startColor = text.color;
                Color endColor = GetParam<Color>(param);
                float timer;
                while (true)
                {
                    timer = 0;
                    while (timer < duration)
                    {
                        text.color = Color.Lerp(startColor, endColor, function(Spike(timer / duration)));
                        timer += Time.deltaTime;
                        yield return null;
                    }
                }
            }
        }

        public static class Custom
        {

        }

        public enum Timer
        {
            Delta,
            Unscalled
        }

        public enum Action
        {
            Straight,
            Force
        }

        private static float TimeScale(Timer timer)
        {
            if (timer == Timer.Delta)
                return Time.deltaTime;
            else
                return Time.unscaledDeltaTime;
        }


        public static Vector3 GetStartPosition(GameObject obj,Space space)
        {
            if (space == Space.World)
            {
                return  obj.transform.position;
            } else
            {
                return  obj.transform.localPosition;
            }
        }

        public static Quaternion GetStartRotation(GameObject obj, Space space)
        {
            if (space == Space.World)
            {
                return obj.transform.rotation;
            }
            else
            {
                return obj.transform.localRotation;
            }
        }

        public static T GetParam<T>(bool isError,params object[] param)
        {
            foreach (var item in param)
            {
                if (item.GetType() == typeof(T))
                {
                    return (T)item;
                }
            }
            if (isError)
            {
                throw new Exception("TWEEN ERROR, add data of type " + typeof(T));
            }
            else return default;
        }

        public static T GetParam<T>(params object[] param)
        {
            foreach (var item in param)
            {
                if (item.GetType() == typeof(T))
                {
                    return (T)item;
                }
            }
            return default;
        }
    }
}