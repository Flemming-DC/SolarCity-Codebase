using System;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

public class ShieldRotator : MonoBehaviour
{
    [SerializeField] float defaultDeInterpolationSpeed = 1;
    //[SerializeField] Transform shieldLocationForRightHand;
    [SerializeField] BlockAnimations blockAnimationClass;
    [SerializeField] InterpolationData[] interpolationDatas;

    AnimancerComponent animancer;
    Transform shieldModel;
    Vector3 initialPosition;
    Quaternion initialRotation;
    float interpolationParameter;
    List<string> blockAnimationList = new List<string>();
    bool isDestroyed;

    private void Start()
    {
        shieldModel = transform.GetChild(0);
        if (!shieldModel.TryGetComponent(out MeshRenderer dummy))
            Debug.LogWarning(
                $"the shield's model is assumed to be the first child of the shield prefab. " +
                $"However, no MeshRenderer was found on the first child, so presumably that" +
                $"child isn't the model for the shield.");
        initialPosition = shieldModel.localPosition;
        initialRotation = shieldModel.localRotation;

        animancer = GetComponent<Weapon>().GetOwnerAtStart().GetComponentInDirectChildren<AnimancerComponent>();
        foreach (var data in interpolationDatas)
            data.Setup();
        blockAnimationList.Add(blockAnimationClass.blockStartClipName);
        blockAnimationList.Add(blockAnimationClass.blockLoopClipName);
        blockAnimationList.Add(blockAnimationClass.blockHitClipName);
    }


    void Update()
    {
        if (animancer == null) // animancer becomes null, when the shield gets destroyed. Note the Update runs on the frame where destroy gets called, even if if is called before update. 
            return;
        if (animancer.States.Current == null)
            return;
        if (isDestroyed)
            return;

        foreach (var data in interpolationDatas)
        {
            if (animancer.States.Current.Clip != data.animationClip)
                continue;

            interpolationParameter = InterpolationParameter(data, animancer.States.Current.NormalizedTime);
            shieldModel.localPosition = Vector3.Lerp(initialPosition, data.finalTransform.localPosition, interpolationParameter);
            shieldModel.localRotation = Quaternion.Lerp(initialRotation, data.finalTransform.localRotation, interpolationParameter);
            return;
        }

        if (animancer.States.Current.Clip == null)
            return;
        if (blockAnimationList.Contains(animancer.States.Current.Clip.name))
            return;

        if (shieldModel.localPosition != initialPosition)
        {
            shieldModel.localPosition = Vector3.Lerp(shieldModel.localPosition, initialPosition, defaultDeInterpolationSpeed * Time.deltaTime);
            shieldModel.localRotation = Quaternion.Lerp(shieldModel.localRotation, initialRotation, defaultDeInterpolationSpeed * Time.deltaTime);
        }

    }


    public void Destroy()
    {
        isDestroyed = true;
        Destroy(this);
    }

    float InterpolationParameter(InterpolationData data, float time)
    {
        if (time < data.interpolationFinishedTime)
            return time / data.interpolationFinishedTime;
        else if (time < data.deInterpolationBeginingTime)
            return 1;
        else
            return (1 - time) / (1 - data.deInterpolationBeginingTime);
    }


    [Serializable]
    class InterpolationData
    {
        public Transform finalTransform;
        public AnimationClip animationClip;
        [SerializeField] float interpolationFinishedFrameNumber;
        [SerializeField] float deInterpolationBeginingFrameNumber;
        [SerializeField] float animationFinishedFrameNumber;


        public float interpolationFinishedTime { get; private set; }
        public float deInterpolationBeginingTime { get; private set; }

        public void Setup()
        {
            interpolationFinishedTime = interpolationFinishedFrameNumber / animationFinishedFrameNumber;
            deInterpolationBeginingTime = deInterpolationBeginingFrameNumber / animationFinishedFrameNumber;
        }

    }



}
