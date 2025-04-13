using UnityEngine;


public class Jump : RootMotionAction
{
    [SerializeField] float bottomDuringJump;
    [SerializeField] float topDuringJump;
    [SerializeField] float horizontalSpeed = 3;
    [SerializeField] float verticalRootMotionMagnifier = 1;

    CapsuleUpdater capsuleUpdater;
    TwoHandingIK twoHandingIK;
    PlayerMotion playerMotion;

    protected override void Start()
    {
        capsuleUpdater = this.GetComponentInSiblings<CapsuleUpdater>();
        twoHandingIK = this.GetComponentInSiblings<TwoHandingIK>();
        playerMotion = GetComponent<PlayerMotion>();
        base.Start();
    }

    public override void OnEnter(object dummy)
    {
        base.OnEnter(null);
        twoHandingIK.activationOverriders.Add(this);
        //Invoke(nameof(SetCapsules), 0.5f);
    }

    public override void Tick()
    {
        base.Tick();
        if (playerMotion == null)
            return;

        Vector2 movementInput = InputManager.gameplay.Walk.ReadValue<Vector2>();
        Vector3 moveDirection = playerMotion.GetMoveDirection(movementInput);
        playerMotion.RotateTowards(moveDirection);
        if (!mover.isGrounded)
            mover.velocity += moveDirection * horizontalSpeed;

    }

    public override void OnExit()
    {
        base.OnExit();
        twoHandingIK.activationOverriders.Remove(this);
        //capsuleUpdater.ResetCapsules();
    }

    protected override Vector3 RootVelocityModifier(Vector3 rootVelocity)
    {
        return rootVelocity * verticalRootMotionMagnifier;
    }

    void SetCapsules()
    {
        capsuleUpdater.SetCapsulesByTop(bottomDuringJump, topDuringJump);
    }

}
