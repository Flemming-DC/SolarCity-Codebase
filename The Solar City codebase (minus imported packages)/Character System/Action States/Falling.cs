using UnityEngine;
using Animancer;

public class Falling : ActionState
{
    public float gravity = 25f;
    public float maxFallingSpeed = 12f;
    [SerializeField] float controllableHorizontalSpeed = 2;
    [SerializeField] float initialHorizontalSpeedMultiplier = 1;
    [Tooltip("Rotating during the fall can serve to undo the effect of root motion rotation from the last state.")] 
    [SerializeField] float rotationSpeed = 500;
    [SerializeField] Transform modelTransform;
    [SerializeField] ClipTransition fallingAnimation;

    public Quaternion? targetRotation { private get; set; }
    public bool canFall { get; set; }
    float gravitationalSpeed;
    Vector3 fallOffVelocity;
    AnimancerComponent animancer;
    ScivoloMover mover;
    TwoHandingIK twoHandingIK;
    PlayerMotion playerMotion;
    
    bool hasLandingState;

    private void Start()
    {
        ActMachine actMachine = this.GetComponentInSiblings<ActMachine>();
        animancer = this.GetComponentInSiblings<AnimancerComponent>();
        twoHandingIK = this.GetComponentInSiblings<TwoHandingIK>();
        mover = this.GetComponentInSiblings<ScivoloMover>();
        hasLandingState = actMachine.states.ContainsKey(typeof(Landing));
        playerMotion = GetComponent<PlayerMotion>();
        inVoluntary = true;

        actMachine.AddTransitionFromAny(this, () => !mover.isGroundedWithLag && canFall);// && actMachine.currentStateType != typeof(AIMotion));
        if (hasLandingState)
            actMachine.AddTransition(this, typeof(Landing), () => mover.isGroundedWithLag);
        else
            actMachine.AddTransition(this, typeof(Idle), () => mover.isGroundedWithLag);

        canFall = true;
    }


    public override void OnEnter(object dummy)
    {
        fallOffVelocity = initialHorizontalSpeedMultiplier * mover.velocity;
        gravitationalSpeed = 0;
        animancer.Play(fallingAnimation, 0.25f, FadeMode.FixedSpeed);
        twoHandingIK.activationOverriders.Add(this);
    }

    public override void Tick()
    {
        if (gravitationalSpeed < maxFallingSpeed)
            gravitationalSpeed += gravity * Time.deltaTime;
        mover.velocity = fallOffVelocity + gravitationalSpeed * Vector3.down;

        if (targetRotation != null)
            transform.parent.rotation = Quaternion.RotateTowards(
                transform.parent.rotation, 
                (Quaternion)targetRotation, 
                rotationSpeed * Time.deltaTime);

        HandleHorizontalMovement();

    }
    public override void OnExit()
    {
        targetRotation = null;
        if (!hasLandingState)
            ResetTwoHanding();
    }

    public void ResetTwoHanding()
    {
        twoHandingIK.activationOverriders.Remove(this);
    }

    void HandleHorizontalMovement()
    {
        Vector3 moveDirection;
        bool isPlayer = (playerMotion != null);

        if (isPlayer)
        {
            Vector2 movementInput = InputManager.gameplay.Walk.ReadValue<Vector2>();
            moveDirection = playerMotion.GetMoveDirection(movementInput);
            playerMotion.RotateTowards(moveDirection);
        }
        else
            moveDirection = modelTransform.forward;

        if (!mover.isGrounded)
            mover.velocity += moveDirection * controllableHorizontalSpeed;
    }

}
