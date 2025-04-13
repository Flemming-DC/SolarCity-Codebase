using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Animancer;

public class PlayerMotion : ActionState, IPlayerMotion
{
    [Header("Move Speeds")]
    [SerializeField] Stat walkSpeed = 2;
    [SerializeField] Stat runSpeed = 5;
    [SerializeField] Stat sprintSpeed = 8;
    [Header("Diverse")]
    [SerializeField] float sprintingStaminaCostPerTime = 0.2f;
    [SerializeField] float rotationSpeed;
    [SerializeField] float walkRunThreshold = 0.4f;
    [SerializeField] Transform cameraFollowTarget;
    [SerializeField] Transform modelTransform;
    [SerializeField] ClipTransition walk;

    Stat moveSpeed;
    enum MoveState { walk, run, sprint};
    MoveState? lastMoveState;
    ClipTransition run;
    ClipTransition sprint;
    ScivoloMover mover;
    AnimancerComponent animancer;
    AnimationUpdater animationUpdater;
    ClipTransition walk_ = new ClipTransition();

    private void Start()
    {
        animancer = this.GetComponentInSiblings<AnimancerComponent>();
        mover = this.GetComponentInSiblings<ScivoloMover>();
        animationUpdater = this.GetComponentInSiblings<AnimationUpdater>(false);
        SetAnimation(animationUpdater.animations);
        interruptable = true;
        if (animationUpdater != null)
            animationUpdater.OnAnimationsChanged += SetAnimation;
    }

    private void OnDestroy()
    {
        if (animationUpdater != null)
            animationUpdater.OnAnimationsChanged -= SetAnimation;
    }

    public override void OnEnter(object dummy) { }

    public override void Tick()
    {
        Vector2 movementInput = InputManager.gameplay.Walk.ReadValue<Vector2>();
        // the comparison with InputActionPhase.Started creates a small delay in 
        // the sprint action, which prevents it from being confused with a dodge.
        bool isSprinting = InputManager.gameplay.Sprint.IsPressed()
            && InputManager.gameplay.Sprint.phase != InputActionPhase.Started;
        if (isSprinting && !Stamina.TryPayStaminaCost(sprintingStaminaCostPerTime * Time.deltaTime))
            isSprinting = false;

        SetSpeedAndAnimation(movementInput, isSprinting);
        Vector3 moveDirection = GetMoveDirection(movementInput);
        RotateTowards(moveDirection);
        mover.velocity = moveDirection * moveSpeed;
    }

    public override void OnExit()
    {
        lastMoveState = null; // this line prevents SetSpeedAndAnimation from bailing out, when the animation starts with the same speed as it had during its last exit.
    }

    MoveState GetMoveState(Vector2 movementInput, bool isSprinting)
    {
        if (movementInput.magnitude <= walkRunThreshold)
            return MoveState.walk;
        else if (!isSprinting)
            return MoveState.run;
        else
            return MoveState.sprint;
    }

    void SetSpeedAndAnimation(Vector2 movementInput, bool isSprinting, bool onlyWhenChanged = true)
    {
        MoveState moveState = GetMoveState(movementInput, isSprinting);
        if (moveState == lastMoveState && onlyWhenChanged)
            return;
        else
            lastMoveState = moveState;


        if (moveState == MoveState.walk)
        {
            moveSpeed = walkSpeed;
            float time = animancer.States.Current.NormalizedTime;
            animancer.Play(walk_, 0.25f).NormalizedTime = time;
        }
        else if (moveState == MoveState.run)
        {
            moveSpeed = runSpeed;
            float time = animancer.States.Current.NormalizedTime;
            animancer.Play(run, 0.25f).NormalizedTime = time;
        }
        else if (moveState == MoveState.sprint)
        {
            moveSpeed = sprintSpeed;
            float time = animancer.States.Current.NormalizedTime;
            animancer.Play(sprint, 0.25f).NormalizedTime = time;
        }
        else
            Debug.LogWarning($"the moveState = {moveState}, but the only acceptable values are walk, run and sprint.");
    }

    public Vector3 GetMoveDirection(Vector2 movementInput)
    {
        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y);
        moveDirection.Normalize();
        return Quaternion.Euler(0, cameraFollowTarget.eulerAngles.y, 0) * moveDirection;
    }

    public void RotateTowards(Vector3 movementDirection)
    {
        if (movementDirection == Vector3.zero)
            return;
        
        Quaternion targetRotation = Quaternion.LookRotation(movementDirection, transform.up);
        modelTransform.rotation = Quaternion.RotateTowards(modelTransform.rotation, 
                                                           targetRotation, 
                                                           rotationSpeed * Time.deltaTime);
    }

    public void ApplySpeedModifier(float factor, float? duration = null)
    {
        walkSpeed.ApplyFactor(factor, duration);
        runSpeed.ApplyFactor(factor, duration);
        sprintSpeed.ApplyFactor(factor, duration);
    }

    void SetAnimation(AnimationsGivenWeaponClass animations)
    {
        bool isTwohanding = false;
        try { isTwohanding = this.GetComponentInSiblings<WeaponHandler>().weapons[Hand.right].IsTwoHanded(); }
        catch { }
        walk_.Clip = isTwohanding ? animations.run.Clip : walk.Clip;
        walk_.Speed = isTwohanding ? walkSpeed / runSpeed : 1;
        
        run = animations.run;
        sprint = animations.sprint;

        Vector2 movementInput = InputManager.gameplay.Walk.ReadValue<Vector2>();
        if (movementInput == Vector2.zero)
            return;

        bool isSprinting = InputManager.gameplay.Sprint.IsPressed();
        SetSpeedAndAnimation(movementInput, isSprinting, false);
    }

}
