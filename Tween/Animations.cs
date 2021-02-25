using UnityEngine;
using System;
using System.Collections;
using static FUGames.Tweeny.Function;

namespace Tweeny
{
    //TweenyAnimations

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
            Rotate,
            SpikeRotate,
            Scaling,
            Scale,
            SpikeScale,
            UnscaledMove,
        }

        //Keep this two similar

        public static AnimationHandler[] Animations = new AnimationHandler[]
        {
                Move,
                PingPong,
                Blinking,
                Rotation,
                Rotate,
                SpikeRotate,
                Scaling,
                Scale,
                SpikeScale,
                UnscaledMove,
        };

        public static IEnumerator Play(AnimationName animation, FunctionName function, float duration, GameObject gameObject, params object[] param)
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

        public static IEnumerator Move(FunctionHandler function, float duration, GameObject obj, params object[] param)
        {
            Vector3 point = GetParam<Vector3>(param);
            Vector3 startPosition = obj.transform.localPosition;
            float timer = 0;
            while (timer <= duration)
            {
                obj.transform.localPosition = Vector3.Lerp(startPosition, point, function(timer / duration));
                timer += Time.deltaTime;
                yield return null;
            }
            //ВОЗМОЖНО СТОИТ ВОЗВРАЩАТЬ НА МЕСТО ТУТ ВО ВСЕХ ФУНЦИЯХ ЕСЛИ НУЖНО
        }

        public static IEnumerator UnscaledMove(FunctionHandler function, float duration, GameObject obj, params object[] param)
        {
            Vector3 point = GetParam<Vector3>(param);
            Vector3 startPosition = obj.transform.localPosition;
            float timer = 0;
            while (timer <= duration)
            {
                obj.transform.localPosition = Vector3.Lerp(startPosition, point, function(timer / duration));
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
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

        public static IEnumerator Rotate(FunctionHandler function, float duration, GameObject obj, params object[] param)
        {
            Quaternion startRotation = obj.transform.localRotation;
            Quaternion endRotation = Quaternion.Euler(GetParam<Vector3>(param));
            float timer;
            timer = 0;
            while (timer < duration)
            {
                obj.transform.localRotation = Quaternion.Lerp(startRotation, endRotation, function(timer / duration));
                timer += Time.deltaTime;
                yield return null;
            }
        }

        public static IEnumerator SpikeRotate(FunctionHandler function, float duration, GameObject obj, params object[] param)
        {
            Quaternion startRotation = obj.transform.localRotation;
            Quaternion endRotation = Quaternion.Euler(GetParam<Vector3>(param));
            float timer;
            timer = 0;
            while (timer < duration)
            {
                obj.transform.localRotation = Quaternion.Lerp(startRotation, endRotation, Spike(function(timer / duration)));
                timer += Time.deltaTime;
                yield return null;
            }
        }

        public static IEnumerator Scaling(FunctionHandler function, float duration, GameObject obj, params object[] param)
        {
            Vector3 startScale = obj.transform.localScale;
            Vector3 endScale = GetParam<Vector3>(param);
            float timer;
            while (true)
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

        public static IEnumerator Scale(FunctionHandler function, float duration, GameObject obj, params object[] param)
        {
            Vector3 startScale = obj.transform.localScale;
            Vector3 endScale = GetParam<Vector3>(param);
            float timer;
            timer = 0;
            while (timer < duration)
            {
                obj.transform.localScale = Vector3.Lerp(startScale, endScale, function(timer / duration));
                timer += Time.deltaTime;
                yield return null;
            }
        }

        public static IEnumerator SpikeScale(FunctionHandler function, float duration, GameObject obj, params object[] param)
        {
            Vector3 startScale = obj.transform.localScale;
            Vector3 endScale = GetParam<Vector3>(param);
            float timer;
            timer = 0;
            while (timer < duration)
            {
                obj.transform.localScale = Vector3.Lerp(startScale, endScale, Spike(timer / duration));
                timer += Time.deltaTime;
                yield return null;
            }
            obj.transform.localScale = startScale;
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
            Debug.LogError("TWEEN ERROR, add data of type " + typeof(T));
            return default;
        }

        private static void SimpleLerp(float duration)
        {
            float timer = 0;

        }
    }
}