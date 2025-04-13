using System.Collections;
using UnityEngine;

public class PlayerRootMotionAction : RootMotionAction
{
    [SerializeField] float rotationSpeed = 700;
    [SerializeField] Transform modelTransform;

    PlayerMotion playerMotion;

    protected override void Start()
    {
        playerMotion = this.GetComponent<PlayerMotion>(true);
        base.Start();
    }

    public override void OnEnter(object inputDirection)
    {
        base.OnEnter(null);
        StartCoroutine(Rotate((Vector2)inputDirection));
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    IEnumerator Rotate(Vector2 inputDirection)
    {
        if (inputDirection == Vector2.zero)
            yield break;

        Vector3 direction = playerMotion.GetMoveDirection(inputDirection);
        Quaternion targetRotation = Quaternion.LookRotation(direction, transform.up);

        while (Quaternion.Angle(modelTransform.rotation, targetRotation) > 0.1f)
        {
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }


}
