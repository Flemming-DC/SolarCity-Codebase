using System.Collections;
using UnityEngine;

public class Dodge : PlayerRootMotionAction
{
    [SerializeField] float heightDuringDodge;
    [SerializeField] float rootVelocityMagnifier = 1;
    [SerializeField] float dodgeRadius = 0.2f;
    [SerializeField] float dodgeMinRadiusDuration;
    [SerializeField] float staminaCost = 0.33f;

    CapsuleUpdater capsuleUpdater;

    protected override void Start()
    {
        capsuleUpdater = this.GetComponentInSiblings<CapsuleUpdater>();
        maxTime = 10;
        base.Start();
    }


    public void TryEnter(Vector2 inputDirection)
    {
        if (Stamina.TryPayStaminaCost(staminaCost))
            actMachine.SetState<Dodge>(inputDirection);
    }

    public override void OnEnter(object inputDirection)
    {
        base.OnEnter(inputDirection);
        capsuleUpdater.SetCapsulesByHeight(capsuleUpdater.bottom, heightDuringDodge);
        StartCoroutine(capsuleUpdater.LerpCapsuleRadius(dodgeRadius, actionAnimation.Clip.length, true, dodgeMinRadiusDuration));
    }

    public override void OnExit()
    {
        base.OnExit();
        capsuleUpdater.ResetCapsules();
    }

    protected override Vector3 RootVelocityModifier(Vector3 rootVelocity)
    {
        return rootVelocityMagnifier * rootVelocity;
    }

}
