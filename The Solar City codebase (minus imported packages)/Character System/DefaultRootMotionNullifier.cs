using System;
using System.Collections.Generic;
using UnityEngine;

public class DefaultRootMotionNullifier : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
        animator = this.GetComponent<Animator>(true);
        animator.applyRootMotion = true;
    }


    private void OnAnimatorMove()
    {
        // denne funktion forhindrer animatoren i at bruge rootmotion uden at den får besked på det.
    }


}
