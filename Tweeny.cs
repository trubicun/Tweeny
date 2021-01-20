using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tweeny.Function;
using static Tweeny.Animation.Transformation;
using UnityEngine.UI;

namespace Tweeny
{
    //Tween GUI
    public class Tweeny : MonoBehaviour
    {
        public static Tweeny Instance;

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

        public void AddObject(MonoBehaviour mono, AnimationName animation, FunctionName function, float duration, GameObject gameObject, params object[] customData)
        {
            TweenObjects.Add(new TweenObject(mono, animation, function, duration, gameObject, customData));
        }

        public void DeleteObject(TweenObject animatedObject)
        {
            TweenObjects.Remove(animatedObject);
        }
    }

    //Tween Object

    [Serializable]
    public class TweenObject
    {
        public Queue<TweenData> Tweens { get; private set; }
        public TweenData CurrentAnimation { get; private set; }


        public TweenObject(TweenData tweenData)
        {
            Tweens = new Queue<TweenData>();
            Tweens.Enqueue(tweenData);
        }

        public TweenObject(MonoBehaviour mono, AnimationName animation, FunctionName function, float duration, GameObject gameObject, params object[] customData)
        {
            Tweens = new Queue<TweenData>();
            Tweens.Enqueue(new TweenData(mono, animation, function, duration, gameObject, customData));
        }

        public TweenObject(MonoBehaviour mono, IEnumerator animation)
        {
            Tweens = new Queue<TweenData>();
            Tweens.Enqueue(new TweenData(mono, animation));
        }

        public void Start()
        {
            if (Tweens.Count > 0)
            {
                CurrentAnimation = Tweens.Dequeue();
                CurrentAnimation.Play();
                CurrentAnimation.AnimationEnd += Start;
            }
        }

        public void Play()
        {
            CurrentAnimation = Tweens.Peek();
            CurrentAnimation.Play();
        }

        public void Stop(MonoBehaviour mono)
        {
            if (CurrentAnimation != null) CurrentAnimation.Stop();
        }

        public void AddAnimation(TweenData tweenData)
        {
            Tweens.Enqueue(tweenData);
        }

        public void AddAnimation(MonoBehaviour mono,AnimationName animation, FunctionName function, float duration, GameObject gameObject, params object[] customData)
        {
            Tweens.Enqueue(new TweenData(mono, animation, function, duration, gameObject, customData));
        }

        public void DeleteAnimation()
        {
            Tweens.Dequeue();
        }

        /*
        //For CustomEditor
        public void SetAnimation(TweenData tweenData)
        {
            TweenData data = Tweens.Dequeue();
            data = new TweenData(tweenData.Animation, tweenData.Function, tweenData.Duration, tweenData.GameObject, tweenData.CustomData);
            Tweens.Enqueue(data);
        }
        */
    }

    public class TweenData
    {
        public MonoBehaviour Mono { get; set; }
        public IEnumerator Animation { get; set; }
        public float Duration { get; set; }
        public GameObject GameObject { get; set; }
        public object[] CustomData { get; set; }

        public delegate void EventHandler();
        public event EventHandler AnimationEnd;


        public TweenData(MonoBehaviour mono, AnimationName animation, FunctionName function, float duration, GameObject gameObject, params object[] customData)
        {
            Mono = mono;
            Animation = Animations[(int)animation](
                        Functions[(int)function], duration, gameObject, customData);
            Duration = duration;
            GameObject = gameObject;
            CustomData = customData;
        }

        public TweenData(MonoBehaviour mono, IEnumerator animation)
        {
            Mono = mono;
            Animation = animation;
        }

        public void Play()
        {
            Mono.StartCoroutine(Animation);
            Mono.StartCoroutine(End(Duration));
        }

        public void Stop()
        {
            Mono.StopCoroutine(Animation);
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