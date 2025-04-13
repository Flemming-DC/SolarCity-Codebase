using System.Collections;
using UnityEngine;
using Animancer;

public class Getup : ActionState
{
    [SerializeField] float getupSpeed = 1;
    [SerializeField] ClipTransitionAsset.UnShared getupFromStomach;
    [SerializeField] ClipTransitionAsset.UnShared getupFromBack;

    AnimancerComponent animancer;
    ActMachine actMachine;
    Ragdoll ragdoll;

    private void Start()
    {
        animancer = this.GetComponentInSiblings<AnimancerComponent>();
        actMachine = this.GetComponentInSiblings<ActMachine>();
        ragdoll = this.GetComponentInSiblings<Ragdoll>();
        maxTime = 10;

        inVoluntary = true;
    }

    public override void OnEnter(object input)
    {
        (Transform animatedHips, KeyFrameData getupData) = ((Transform, KeyFrameData))input;
        StartCoroutine(GetUpRoutine(animatedHips, getupData));
    }

    public override void OnExit()
    {
        ragdoll.isToppled = false;
    }

    IEnumerator GetUpRoutine(Transform animatedHips, KeyFrameData getUpData)
    {
        animancer.Animator.enabled = false;
        StartCoroutine(getUpData.LerpRoutine(CombatSettings.instance.ragdollToGetupLerpDuration / getupSpeed));
        yield return new WaitForSeconds(CombatSettings.instance.ragdollToGetupLerpDuration / getupSpeed);
        animancer.Animator.enabled = true;

        var animState = animancer.Play(GetAnimation(animatedHips));
        animState.Speed *= getupSpeed;
        float normalizedToppleFinishedTime = NormalizedToppleFinishedTime(animatedHips);
        yield return new WaitUntil(() => animState.NormalizedTime >= normalizedToppleFinishedTime);
        ragdoll.isToppled = false;
        yield return new WaitUntil(() => animState.NormalizedTime >= animState.NormalizedEndTime);

        actMachine.SetNextState(typeof(Idle));
    }

    public ClipTransitionAsset.UnShared GetAnimation(Transform animatedHips)
    {
        if (IsOnBack(animatedHips))
            return getupFromBack;
        else
            return getupFromStomach;
    }

    float NormalizedToppleFinishedTime(Transform animatedHips)
    {
        if (IsOnBack(animatedHips))
            return CombatSettings.instance.normalizedToppleFinishedTime_GettingUpFromBack;
        else
            return CombatSettings.instance.normalizedToppleFinishedTime_GettingUpFromStomach;
    }

    bool IsOnBack(Transform hips)
    {
        return Vector3.Dot(hips.forward, Vector3.up) > 0;
    }




}
