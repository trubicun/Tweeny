using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FUGames.Tween.Function;
using static FUGames.Tween.Animation;

namespace FUGames
{
    namespace Tween
    {
        public class Tween : MonoBehaviour
        {
            public static Tween Instance;

            public List<TweenObject> TweenObjects = new List<TweenObject>();

            public List<string> FunctionNames { get; private set; }
            public List<string> AnimationNames { get; private set; }

            private void Awake() => Instance = this;

            private void Start()
            {
                foreach (TweenObject tweenObject in TweenObjects)
                {
                    tweenObject.Start();
                }
            }

            public void AddObject(TweenData tweenData)
            {
                //TweenObjects.Add(new TweenObject(tweenData));
            }

            public void DeleteObject(TweenObject animatedObject)
            {
                TweenObjects.Remove(animatedObject);
            }

            [Serializable]
            public class TweenObject
            {
                public List<TweenData> Tweens { get; private set; }

                public TweenObject(AnimationName animation, FunctionName function, float duration, GameObject gameObject, params object[] customData)
                {
                    Tweens = new List<TweenData>();
                    Tweens.Add(new TweenData(animation,function,duration,gameObject,customData));
                    Start();
                }

                public void Start()
                {
                    Tweens[0].Play();
                }

                public void AddAnimation(AnimationName animation, FunctionName function, float duration, GameObject gameObject, params object[] customData)
                {
                    Tweens.Add(new TweenData(animation,function,duration,gameObject,customData));
                }

                public void DeleteAnimation(TweenData tweenData)
                {
                    Tweens.Remove(tweenData);
                }
               

                //For CustomEditor
                public void SetAnimation(int index, TweenData tweenData)
                {
                    Tweens[index] = tweenData;
                }

            }

            public class TweenData
            {
                public AnimationName Animation { get; private set; }
                public FunctionName Function { get; private set; }
                public float Duration { get; private set; }
                public GameObject GameObject { get; private set; }

                public object[] CustomData { get; private set; }

                private IEnumerator coroutine;

                public TweenData(AnimationName animation, FunctionName function, float duration, GameObject gameObject, params object[] customData)
                {
                    Animation = animation;
                    Function = function;
                    Duration = duration;
                    GameObject = gameObject;
                    CustomData = customData;
                }

                public void Play()
                {
                    coroutine = Animations[(int)Animation](
                    Functions[(int)Function], Duration, GameObject, CustomData);
                    Instance.StartCoroutine(coroutine);
                }
            }
        }

        [Serializable]
        public static class Function
        {
            //The higher value - the higher Smoothness animation, but higher calculation
            private static int _smoothness = 2;

            public delegate float FunctionHandler(float val);


            //Information about Functions https://www.febucci.com/2018/08/easing-functions/

            //Start Animation via code
            //FUGames.Tween.Animation.Animations[2](args);
            //StartCoroutine(FUGames.Tween.Animation.PingPong(args));

            public enum FunctionName
            {
                Linear,
                EaseIn,
                Flip,
                EaseOut,
                EaseInOut,
                Spike
            }

            //Keep this two similar

            public static FunctionHandler[] Functions = new FunctionHandler[]
            {
                Linear,
                EaseIn,
                Flip,
                EaseOut,
                EaseInOut,
                Spike,
            };


            public static float Linear(float val)
            {
                return val;
            }
            public static float EaseIn(float val)
            {
                return Mathf.Pow(val, _smoothness);
            }
            public static float Flip(float val)
            {
                return 1 - val;
            }
            public static float EaseOut(float val)
            {
                return Flip(Mathf.Pow(Flip(val), _smoothness));
            }
            public static float EaseInOut(float val)
            {
                return Mathf.Lerp(EaseIn(val), EaseOut(val), val);
            }
            public static float Spike(float val)
            {
                if (val <= .5f)
                    return Linear(val / .5f);

                return Linear(Flip(val) / .5f);
            }
        }

        [Serializable]
        public static class Animation
        {
            public delegate IEnumerator AnimationHandler(FunctionHandler function, float duration, GameObject gameObject, params object[] param);

            public enum AnimationName
            {
                Move,
                PingPong,
                Blinking,
                Rotation,
                Scaling
            }

            //Keep this two similar

            public static AnimationHandler[] Animations = new AnimationHandler[]
            {
                Move,
                PingPong,
                Blinking,
                Rotation,
                Scaling
            };

            public static IEnumerator Play(AnimationName animation, FunctionName function, float duration,GameObject gameObject, params object[] param)
            {
                IEnumerator enumerator = Animations[(int)animation](
                    Functions[(int)function], duration, gameObject, param);
                Tween.Instance.StartCoroutine(enumerator);
                return enumerator;
            }

            public static void Stop(IEnumerator coroutine)
            {
                Tween.Instance.StopCoroutine(coroutine);
            }

            public static void Stop(string name)
            {
                Tween.Instance.StopCoroutine(name);
            }

            public static IEnumerator Move(FunctionHandler function, float duration, GameObject obj, params object[] param)
            {
                Vector3 point = GetParam<Vector3>(param);
                Vector3 startPosition = obj.transform.localPosition;
                float timer = 0;
                while (timer <= duration)
                {
                    obj.transform.localPosition = Vector3.Lerp(startPosition,point,function(timer/duration));
                    timer += Time.deltaTime;
                    yield return null;
                }
                //ВОЗМОЖНО СТОИТ ВОЗВРАЩАТЬ НА МЕСТО ТУТ
            }

            public static IEnumerator PingPong(FunctionHandler function, float duration, GameObject obj, params object[] param)
            {
                while (true)
                {
                    Vector3 point = GetParam<Vector3>(param);
                    Vector3 startPosition = obj.transform.localPosition;
                    float timer = 0;
                    while (timer <= duration)
                    {
                        obj.transform.localPosition = Vector3.Lerp(startPosition, point, (function(timer / duration)));
                        timer += Time.deltaTime;
                        yield return null;
                    }
                    timer = 0;
                    while (timer <= duration)
                    {
                        obj.transform.localPosition = Vector3.Lerp(point, startPosition, (function(timer / duration)));
                        timer += Time.deltaTime;
                        yield return null;
                    }
                }
            }

            public static IEnumerator Blinking(FunctionHandler function, float duration, GameObject obj, params object[] param)
            {
                UnityEngine.UI.Text text = obj.GetComponent<UnityEngine.UI.Text>();
                Color startColor = text.color;
                Color endColor = GetParam<Color>(param);
                float timer;
                while(true)
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

            public static IEnumerator Rotation(FunctionHandler function, float duration, GameObject obj, params object[] param)
            {
                Quaternion startRotation = obj.transform.localRotation;
                Quaternion endRotation = Quaternion.Euler(GetParam<Vector3>(param));
                float timer;
                while (true)
                {
                    timer = 0;
                    while (timer < duration)
                    {
                        obj.transform.localRotation = Quaternion.Lerp(startRotation, endRotation, function(Spike(timer / duration)));
                        timer += Time.deltaTime;
                        yield return null;
                    }
                }
            }

            public static IEnumerator Scaling(FunctionHandler function, float duration, GameObject obj, params object[] param)
            {
                Vector3 startScale = obj.transform.localScale;
                Vector3 endScale = GetParam<Vector3>(param);
                float timer;
                while(true)
                {
                    timer = 0;
                    while (timer < duration)
                    {
                        obj.transform.localScale = Vector3.Lerp(startScale, endScale, function(Spike(timer / duration)));
                        timer += Time.deltaTime;
                        yield return null;
                    }
                }
            }

            private static T GetParam<T>(params object[] param)
            {
                foreach (var item in param)
                {
                    if (item.GetType() == typeof(T))
                    {
                        return (T) item;
                    }
                }
                Debug.LogWarning("TWEEN ERROR, no such parameter: " + typeof(T));
                return default;
            }
        }
    }
}