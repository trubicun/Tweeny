using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Tweeny
{
    //TweenyFunctions

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

        //Keep this names similar

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
}