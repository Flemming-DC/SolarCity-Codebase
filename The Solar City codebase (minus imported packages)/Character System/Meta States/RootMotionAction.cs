using System;
using UnityEngine;
using Animancer;
using ConditionalField;

public abstract class RootMotionAction : ActionState
{
    [SerializeField] bool canFall;
    [ConditionalField(nameof(canFall))] public bool resetRotationWhenFalling;
    [Tooltip("set to 0, in order to use the normalizedEndTime as the normalizedUngroundedEndTime.")]
    [SerializeField] float normalizedUngroundedEndTime = 1;
    //[SerializeField] bool chooseAnimationInInspector;
    //[ConditionalField(nameof(chooseAnimationInInspector))] 
    [SerializeField] protected ClipTransition actionAnimation;

    Transform characterTransform;
    protected AnimancerComponent animancer;
    protected ActMachine actMachine;
    protected ScivoloMover mover;
    Falling falling;
    float gravitationalSpeed;
    Quaternion initialRotation;

    protected virtual void Start()
    {
        characterTransform = transform.parent;
        actMachine = this.GetComponentInSiblings<ActMachine>();
        animancer = this.GetComponentInSiblings<AnimancerComponent>();
        mover = this.GetComponentInSiblings<ScivoloMover>();
        falling = (Falling)actMachine.states[typeof(Falling)];

        if (normalizedUngroundedEndTime > 1)
            Debug.LogWarning($"normalizedUngroundedEndTime should be less than or equal to one");
    }

    public override void OnEnter(object dummy)
    {
        falling.canFall = false; // this script implements its own custom falling logic and therefore it will turn off the default falling logic.
        gravitationalSpeed = 0;
        initialRotation = transform.rotation;
        animancer.Play(actionAnimation);
    }

    public override void Tick()
    {
        if (gravitationalSpeed < falling.maxFallingSpeed && canFall)
            gravitationalSpeed += falling.gravity * Time.deltaTime;

        mover.velocity = RootVelocityModifier(animancer.Animator.deltaPosition / Time.deltaTime) + gravitationalSpeed * Vector3.down;
        characterTransform.rotation *= animancer.Animator.deltaRotation;
        
        SwitchStateIfNeeded();
    }

    public override void OnExit()
    {
        falling.canFall = true;
    }


    protected virtual Vector3 RootVelocityModifier(Vector3 rootVelocity)
    {
        return rootVelocity;
    }

    protected virtual void SwitchStateIfNeeded()
    {
        if (animancer.IsFinished())
            actMachine.SetNextState(typeof(Idle));
        else if (!mover.isGroundedWithLag && animancer.IsFinished(normalizedUngroundedEndTime))
        {
            actMachine.SetNextState(falling);
            if (resetRotationWhenFalling)
                falling.targetRotation = initialRotation;
        }
    }

}
