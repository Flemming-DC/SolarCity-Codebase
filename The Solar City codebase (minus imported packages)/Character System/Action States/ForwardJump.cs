using System.Collections;
using UnityEngine;

public class ForwardJump : PlayerRootMotionAction
{
    [SerializeField] float builtinRootMotionSpeed = 5f;
    [SerializeField] float minimumMagnifier = 0.5f;
    [SerializeField] float bottomDuringJump;
    [SerializeField] float topDuringJump;
    [SerializeField] float verticalRootMotionMagnifier = 1;

    float magnifier;
    TwoHandingIK twoHandingIK;
    CapsuleUpdater capsuleUpdater;

    protected override void Start()
    {
        capsuleUpdater = this.GetComponentInSiblings<CapsuleUpdater>();
        twoHandingIK = this.GetComponentInSiblings<TwoHandingIK>();
        base.Start();
    }

    public override void OnEnter(object inputDirection)
    {
        base.OnEnter(inputDirection);
        twoHandingIK.activationOverriders.Add(this);
        //Invoke(nameof(SetCapsules), 0.1f);
        magnifier = Mathf.Max(mover.velocity.magnitude / builtinRootMotionSpeed, minimumMagnifier);
    }
    public override void OnExit()
    {
        base.OnExit();
        twoHandingIK.activationOverriders.Remove(this);
        //capsuleUpdater.ResetCapsules();
    }

    protected override Vector3 RootVelocityModifier(Vector3 rootVelocity)
    {
        return rootVelocity.With(y: rootVelocity.y * verticalRootMotionMagnifier) * magnifier;
    }

    void SetCapsules()
    {
        capsuleUpdater.SetCapsulesByTop(bottomDuringJump, topDuringJump);
    }





}
