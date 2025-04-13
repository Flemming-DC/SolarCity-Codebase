using System;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

public class Hurt : ActionState
{
    public Sound hurtSound;
    [SerializeField] bool hardToStagger;
    [SerializeField] Pushable pushable = new Pushable(true, false, true, false);
    [SerializeField] float inertia = 1;

    public static event Action<GameObject, GameObject> onHurt; // <attacked, attacker>
    AnimancerComponent animancer;
    AnimationUpdater animationUpdater;
    ActMachine actMachine;
    ScivoloMover mover;
    Ragdoll ragdoll;
    bool canbePushed;
    StaggerData staggerData;
    Dictionary<HurtType, ClipTransition> hurtAnimations = new Dictionary<HurtType, ClipTransition>();

    protected virtual void Start()
    {
        animationUpdater = this.GetComponentInSiblings<AnimationUpdater>(false);
        SetAnimation(animationUpdater.animations);
        if (animationUpdater != null)
            animationUpdater.OnAnimationsChanged += SetAnimation;
        animancer = this.GetComponentInSiblings<AnimancerComponent>();
        actMachine = this.GetComponentInSiblings<ActMachine>();
        mover = this.GetComponentInSiblings<ScivoloMover>();
        ragdoll = this.GetComponentInSiblings<Ragdoll>();
        hurtSound?.MakeSource(gameObject);
        inVoluntary = true;
        maxTime = 10;
    }

    private void OnDestroy()
    {
        if (animationUpdater != null)
            animationUpdater.OnAnimationsChanged -= SetAnimation;
    }

    public override void OnEnter(object staggerData_)
    {
        staggerData = (StaggerData)staggerData_;
        onHurt?.Invoke(transform.parent.gameObject, staggerData.attacker);
        hurtSound?.Play(gameObject);
        canbePushed = CanbePushed();
        mover.velocity = Vector3.zero;

        if (staggerData.TryPush() && canbePushed)
        {
            mover.velocity = mover.velocity.y * Vector3.up + staggerData.push / inertia;
            StartCoroutine(ragdoll.ToppleRoutine());
        }
        else
            animancer.Play(hurtAnimations[staggerData.hurtType]);
    }

    public override void Tick()
    {
        if (staggerData.TryPush() && canbePushed)
            return;

        if (animancer.IsFinished())
            actMachine.SetNextState(typeof(Idle));
    }

    public bool TryEnter(StaggerData staggerData_)
    {
        staggerData = staggerData_;
        if (staggerData.hurtType == HurtType.noReaction)
            return false;
        if (hardToStagger)
        {
            if (staggerData.hurtType == HurtType.normal)
                return false;
            if (staggerData.TryPush() && !CanbePushed())
                return false;
        }

        actMachine.SetNextState(this, null, staggerData_);
        return true;
    }

    bool CanbePushed()
    {
        if (pushable.always)
            return true;
        if (pushable.ifStronglyPushed)
            if (staggerData.hurtType == HurtType.stronglyPushed)
                return true;
        if (pushable.duringWindup)
            if (staggerData.attackProggresion == AttackProggresion.windup)
                return true;
        if (pushable.whenStaggered)
            if (staggerData.lastState == typeof(Hurt))
                return true;

        return false;
    }

    void SetAnimation(AnimationsGivenWeaponClass animations)
    {
        hurtAnimations[HurtType.normal] = animations.hurt;
        hurtAnimations[HurtType.veryHurt] = animations.veryHurt;
        hurtAnimations[HurtType.pushed] = animations.hurt;
        hurtAnimations[HurtType.stronglyPushed] = animations.veryHurt;
    }



    [Serializable]
    struct Pushable
    {
        // can be pushed if at least one of these conditions are satisfied
        // add from behind
        // distinguish between knockback and topling
        public bool duringWindup;
        public bool whenStaggered;
        public bool ifStronglyPushed;
        public bool always;

        public Pushable(bool duringWindup_, bool whenStaggered_, bool ifStronglyPushed_, bool always_)
        {
            duringWindup = duringWindup_;
            whenStaggered = whenStaggered_;
            ifStronglyPushed = ifStronglyPushed_;
            always = always_;
        }
    }

    public struct StaggerData
    {
        public HurtType hurtType;
        public Vector3 push;
        public Type lastState;
        public AttackProggresion attackProggresion;
        public GameObject attacker; // only used by onHurt event

        public StaggerData(HurtType hurtType_, Vector3 push_, Type lastState_, AttackProggresion attackProggresion_, GameObject attacker_)
        {
            hurtType = hurtType_;
            push = push_;
            lastState = lastState_;
            attackProggresion = attackProggresion_;
            attacker = attacker_;
        }

        public bool TryPush() => hurtType == HurtType.pushed || hurtType == HurtType.stronglyPushed;
    }
}

