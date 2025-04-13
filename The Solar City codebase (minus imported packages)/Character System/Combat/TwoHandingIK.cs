using System;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

public class TwoHandingIK : MonoBehaviour
{
    [SerializeField] float interpolationDuration = 0.25f;

    float activation;
    float interpolationTime;
    float initialActivation;
    int targetActivation;
    Animator animator;
    Transform leftHandGrip;
    public List<Component> activationOverriders { get; set; } = new List<Component>();

    private void Start()
    {
        AnimancerComponent animancer = this.GetComponent<AnimancerComponent>(true);
        animator = animancer.Animator;
        animancer.Layers[0].ApplyAnimatorIK = true;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (activation == 0 && targetActivation == 0)
            return;
        if (activationOverriders.Count > 0)
            return;
        if (leftHandGrip == null)
            return;

        interpolationTime += Time.deltaTime / interpolationDuration;
        activation = Mathf.Lerp(initialActivation, targetActivation, interpolationTime);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, activation);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandGrip.position);
    }

    public bool ActivateIK(bool active_, Transform leftHandGrip_ = null)
    {
        interpolationTime = 0;
        initialActivation = activation;
        bool wasPreviouslyActive = (targetActivation == 1);
        targetActivation = active_.ToInt();
        if (active_)
            leftHandGrip = leftHandGrip_;
        return wasPreviouslyActive;
    }


}
