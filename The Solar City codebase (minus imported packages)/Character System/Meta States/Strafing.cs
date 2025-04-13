using System;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

public class Strafing : ActionState
{
    [SerializeField] float strafingSpeed = 5;
    [SerializeField] float rotationSpeed = 700;
    [SerializeField] float strafingRotationSpeed = 10;
    [SerializeField] Transform modelTransform;

    MixerTransition2D strafingMixer;
    ScivoloMover mover;
    ActMachine actMachine;
    AnimationUpdater animationUpdater;
    AnimancerComponent animancer;
    protected Func<Vector2> getInputDirection;
    protected Func<Transform> getTarget;
    MixerState<Vector2> mixerState;

    public Vector2 moveRelativeToLookDirection
    {
        get => mixerState.Parameter;
        set => mixerState.Parameter = value;
    }

    private void Start()
    {
        mover = this.GetComponentInSiblings<ScivoloMover>();
        animancer = this.GetComponentInSiblings<AnimancerComponent>();
        actMachine = this.GetComponentInSiblings<ActMachine>();
        animationUpdater = this.GetComponentInSiblings<AnimationUpdater>(false);
        SetAnimation(animationUpdater.animations);
        strafingMixer = animationUpdater.animations.strafingMixer;
        interruptable = true;
        if (animationUpdater != null)
            animationUpdater.OnAnimationsChanged += SetAnimation;
    }

    private void OnDestroy()
    {
        if (animationUpdater != null)
            animationUpdater.OnAnimationsChanged -= SetAnimation;
    }


    public override void OnEnter(object input)
    {
        if (mover == null)
            Start();
        mixerState = (MixerState<Vector2>)animancer.Play(strafingMixer);
    }

    public override void Tick()
    {
        if (getTarget() == null)
        {
            if (transform.parent.TryGetComponent(out PlayerReferencer _))
                actMachine.SwitchMotionState(actMachine.states[typeof(PlayerMotion)]);
            else
                actMachine.SwitchMotionState(actMachine.states[typeof(AIMotion)]);
            return;
        }

        Vector2 inputDirection = getInputDirection();
        Vector2 newMoveDirection = Vector2.Lerp(moveRelativeToLookDirection, inputDirection, strafingRotationSpeed * Time.deltaTime);
        try
        {
            moveRelativeToLookDirection = newMoveDirection;
        }
        catch (Exception e)
        {
            actMachine.SetNextState(typeof(Idle));
            Debug.LogWarning(
                $"failed to set moveRelativeToLookDirection in strafing on {transform.parent.Path()}\n"
                + $"Time.timeSinceLevelLoad = {Time.timeSinceLevelLoad}\n"
                + e.Message + "\n"
                + e.StackTrace + "\n"
                + $"updating moveRelativeToLookDirection to {newMoveDirection}");
        }

        RotateLookDirection();
        mover.velocity = GetMoveDirection(inputDirection) * strafingSpeed;
    }

    public override void OnExit() { }


    void RotateLookDirection()
    {
        Vector3 targetDirection = (getTarget().position - transform.position).With(y: 0);
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        modelTransform.rotation = Quaternion.RotateTowards(modelTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    Vector3 GetMoveDirection(Vector2 movementInput)
    {
        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y);
        moveDirection.Normalize();
        return modelTransform.rotation * moveDirection;
    }

    void SetAnimation(AnimationsGivenWeaponClass animations)
    {
        strafingMixer = animations.strafingMixer;
        if (actMachine.statemachine.currentState == this)
            animancer.Play(strafingMixer);
    }


}
