using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

public class WeaponPoseIK : MonoBehaviour
{
    [SerializeField] float interpolationDuration = 0.25f;
    [SerializeField] Vector3 rightHandEulerAnges = new Vector3(342.67f, 3.45f, 263.8f);

    float activation;
    float interpolationTime;
    float initialActivation;
    int targetActivation;
    Animator animator;
    TwoHandingIK twoHandingIK;
    Quaternion rightHandRotation;

    private void Start()
    {
        AnimancerComponent animancer = this.GetComponent<AnimancerComponent>(true);
        twoHandingIK = this.GetComponent<TwoHandingIK>(true);
        animator = animancer.Animator;
        animancer.Layers[0].ApplyAnimatorIK = true;
        rightHandRotation = Quaternion.Euler(rightHandEulerAnges);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ActivateIK(true);
            print("activating IK");
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            ActivateIK(false);
            print("deactivating IK");
        }

    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (activation == 0 && targetActivation == 0)
            return;

        interpolationTime += Time.deltaTime / interpolationDuration;
        activation = Mathf.Lerp(initialActivation, targetActivation, interpolationTime);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, activation);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, activation);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandRotation);
    }

    public bool ActivateIK(bool active_)
    {
        interpolationTime = 0;
        initialActivation = activation;
        UpdateTwoHandingIK(active_);
        bool wasPreviouslyActive = (targetActivation == 1);
        targetActivation = active_.ToInt();
        return wasPreviouslyActive;
    }



    void UpdateTwoHandingIK(bool overrideTwoHanding)
    {
        bool alreadyApplied = twoHandingIK.activationOverriders.Contains(this);

        if (overrideTwoHanding && !alreadyApplied)
            twoHandingIK.activationOverriders.Add(this);
        else if (!overrideTwoHanding && alreadyApplied)
            twoHandingIK.activationOverriders.Remove(this);
    }


}
