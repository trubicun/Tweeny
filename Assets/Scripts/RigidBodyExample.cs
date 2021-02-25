﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tweeny.Animation;
using static Tweeny.Animation.RigidBody;
using static Tweeny.Function;

public class RigidBodyExample : MonoBehaviour
{
    [SerializeField] private Vector3 vector;
    [SerializeField] private Action action;
    [SerializeField] float duration = 1f;
    [SerializeField] FunctionName function;
    IEnumerator anim;
    /*
     * 1) StartCoroutine(Move(EaseInOut, 1f, gameObject, Vector3.zero));
     * 2) Ienumarator anim = StartCoroutine(Move(EaseInOut, 1f, gameObject, Vector3.zero));
     * 3) TweenObject tweenObject = new TweenObject(this, AnimationName.Move, FunctionName.EaseInOut,1f,gameObject, Vector3.zero);
     * 4) TweenObject tweenObject = new TweenObject(this,Move(EaseInOut, 1f, gameObject, Vector3.zero));
     * 5) TweenObject tweenObject = new TweenObject(tweenData);
    */
    private void Start()
    {
        anim = Move(Functions[(int)function], duration, gameObject, vector, action);
        StartCoroutine(anim);
    }

    private void Update()
    {
        if (anim.Current.GetType() == typeof(bool))
        {
            anim = Move(Functions[(int)function], duration, gameObject, vector, action);
            StartCoroutine(anim);
        }
    }
}
