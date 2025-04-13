using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

[DefaultExecutionOrder(10)]
public class Idle : ActionState
{
    ClipTransition idleAnimation;
    AnimancerComponent animancer;
    ScivoloMover mover;
    AnimationUpdater animationUpdater;
    ActMachine actMachine;

    private void Awake()
    {
        animancer = this.GetComponentInSiblings<AnimancerComponent>();
        mover = this.GetComponentInSiblings<ScivoloMover>();
        actMachine = this.GetComponentInSiblings<ActMachine>();
        animationUpdater = this.GetComponentInSiblings<AnimationUpdater>(false);
        interruptable = true;
        idleAnimation = CombatSettings.instance.universalDefaultAnimations.idle;
    }

    void OnEnable()
    {
        if (animationUpdater != null)
            animationUpdater.OnAnimationsChanged += SetAnimation;

        // this fixes bug a bug, which arises from disabling and reenabling characters
        SetAnimation(animationUpdater.animations);
        actMachine.SetNextState(typeof(Idle));
        animancer.Play(idleAnimation, 0.25f);
    }

    void OnDisable()
    {
        if (animationUpdater != null)
            animationUpdater.OnAnimationsChanged -= SetAnimation;
    }

    public override void OnEnter(object dummy)
    {
        if (idleAnimation == null)
            Awake();

        mover.velocity = Vector3.zero;
        animancer.Play(idleAnimation, 0.25f);
    }

    public override void Tick() { }
    public override void OnExit() { }


    void SetAnimation(AnimationsGivenWeaponClass animations)
    {
        idleAnimation = animations.idle;
        if (actMachine.statemachine != null)
            if (actMachine.statemachine.currentState == this)
                animancer.Play(idleAnimation, 0.25f);
    }

}
