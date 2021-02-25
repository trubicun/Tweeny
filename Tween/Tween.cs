using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tweeny.Function;
using static Tweeny.Animation;
using UnityEngine.UI;

namespace Tweeny
{
    //Tween GUI
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

        public void AddObject(AnimationName animation, FunctionName function, float duration, GameObject gameObject, params object[] customData)
        {
            TweenObjects.Add(new TweenObject(animation, function, duration, gameObject, customData));
        }

        public void DeleteObject(TweenObject animatedObject)
        {
            TweenObjects.Remove(animatedObject);
        }
    }

    [Serializable]
    public class TweenObject : MonoBehaviour
    {
        public Queue<TweenData> Tweens { get; private set; }
        public TweenData CurrentAnimation { get; private set; }

        public TweenObject(AnimationName animation, FunctionName function, float duration, GameObject gameObject, params object[] customData)
        {
            Tweens = new Queue<TweenData>();
            Tweens.Enqueue(new TweenData(animation, function, duration, gameObject, customData));
        }

        public void Start()
        {
            if (Tweens.Count > 0)
            {
                CurrentAnimation = Tweens.Dequeue();
                CurrentAnimation.Play(this);
                CurrentAnimation.AnimationEnd += Start;
            }
        }

        public void Play()
        {
            CurrentAnimation = Tweens.Peek();
            CurrentAnimation.Play(this);
        }

        public void Stop()
        {
            if (CurrentAnimation != null) CurrentAnimation.Stop(this);
        }

        public void AddAnimation(AnimationName animation, FunctionName function, float duration, GameObject gameObject, params object[] customData)
        {
            Tweens.Enqueue(new TweenData(animation, function, duration, gameObject, customData));
        }

        public void DeleteAnimation()
        {
            Tweens.Dequeue();
        }

        //For CustomEditor
        public void SetAnimation(TweenData tweenData)
        {
            TweenData data = Tweens.Dequeue();
            data = new TweenData(tweenData.Animation, tweenData.Function, tweenData.Duration, tweenData.GameObject, tweenData.CustomData);
            Tweens.Enqueue(data);
        }

    }

    public class TweenData
    {
        public AnimationName Animation { get; private set; }
        public FunctionName Function { get; private set; }
        public float Duration { get; private set; }
        public GameObject GameObject { get; private set; }
        public object[] CustomData { get; private set; }

        public delegate void EventHandler();
        public event EventHandler AnimationEnd;

        private IEnumerator animation;

        public TweenData(AnimationName animation, FunctionName function, float duration, GameObject gameObject, params object[] customData)
        {
            Animation = animation;
            Function = function;
            Duration = duration;
            GameObject = gameObject;
            CustomData = customData;
        }

        public void Play(MonoBehaviour mono)
        {
            animation = Animations[(int)Animation](
            Functions[(int)Function], Duration, GameObject, CustomData);
            mono.StartCoroutine(animation);
            mono.StartCoroutine(End(Duration));
        }

        public void Stop(MonoBehaviour mono)
        {
            mono.StopCoroutine(animation);
        }

        private IEnumerator End(float time)
        {
            float timer = 0;
            while (timer < time)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            AnimationEnd?.Invoke();
        }
    }

    public static class RandomUtilities
    {
        public static IEnumerator UnscaledImageChangeColor(FunctionHandler function, float duration, Image image, Color color)
        {
            Color startColor = image.color;
            float timer = 0;
            while (timer <= duration)
            {
                image.color = Color.Lerp(startColor, color, function(timer / duration));
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            //ВОЗМОЖНО СТОИТ ВОЗВРАЩАТЬ НА МЕСТО ТУТ ВО ВСЕХ ФУНЦИЯХ ЕСЛИ НУЖНО
        }
    }
}