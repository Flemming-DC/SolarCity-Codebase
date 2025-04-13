using UnityEngine;
using UnityEngine.UI;

public class LockOnRotator : ALockOnRotator
{
    [SerializeField] float lockOnAimUIHeight = 1.5f;
    [SerializeField] float lockOnLookAtHeight = 2f;
    [SerializeField] float lockOnSmoothing = 0.03f;
    [SerializeField] float errorMargin = 0.05f;
    [SerializeField] Transform lockOnFollowTarget;
    [SerializeField] Image lockOnIcon;

    public override bool lockOn { get; protected set; }
    public override Transform target { get; protected set; }
    JoystickCameraRotator joystickRotator;
    Transform followTarget;
    LockOnTargeter targeter;
    Vector2 lastSwitchTargetDirection;
    float normalFollowTargetHeight;
    float lockOnFollowTargetHeight;
    float eulerVelocityX;
    float eulerVelocityY;
    Camera mainCamera;

    private void Start()
    {
        joystickRotator = this.GetComponent<JoystickCameraRotator>(true);
        followTarget = joystickRotator.followTarget;
        targeter = this.GetComponent<LockOnTargeter>(true);
        targeter.lockOnLookAtHeight = lockOnLookAtHeight;
        normalFollowTargetHeight = followTarget.localPosition.y;
        lockOnFollowTargetHeight = lockOnFollowTarget.localPosition.y;
        mainCamera = Camera.main;

        InputManager.gameplay.LockOn.performed += dummy => ToggleLockOn();
        InputManager.gameplay.SwitchLockOnTarget.performed += ctx => SwitchTargetIfLockedOn(ctx.ReadValue<Vector2>());
        InputManager.gameplay.SwitchLockOnTarget.canceled += ctx => SwitchTargetIfLockedOn(Vector2.zero);
    }


    private void Update()
    {
        if (lockOn && target == null)
        {
            ToggleLockOn();
            return;
        }
        if (lockOn)
            FaceTarget(target);
    }


    void ToggleLockOn()
    {
        lockOn = !lockOn;
        lockOnIcon.enabled = lockOn;

        if (lockOn)
        {
            target = targeter.FindInitialTarget(followTarget.forward);
            if (target == null)
            {
                lockOn = false;
                lockOnIcon.enabled = false;
                return;
            }
            followTarget.localPosition = followTarget.localPosition.With(y: lockOnFollowTargetHeight);
        }
        else
        {
            joystickRotator.rotationAroundX = followTarget.localRotation.eulerAngles.x;
            joystickRotator.rotationAroundY = followTarget.localRotation.eulerAngles.y;
            followTarget.localPosition = followTarget.localPosition.With(y: normalFollowTargetHeight);
        }
    }

    void SwitchTargetIfLockedOn(Vector2 direction)
    {
        if (!lockOn)
            return;

        if (Vector3.Dot(lastSwitchTargetDirection, direction) > 0)
            return;
        lastSwitchTargetDirection = direction;
        if (Mathf.Abs(direction.sqrMagnitude) < errorMargin * errorMargin)
            return;
        
        target = targeter.SwitchTarget(followTarget, direction);
    }

    void FaceTarget(Transform target_)
    {
        lockOnIcon.rectTransform.position = mainCamera.WorldToScreenPoint(target_.position + lockOnAimUIHeight * Vector3.up);

        Vector3 targetDirection = Quaternion.LookRotation(target_.position + lockOnLookAtHeight * Vector3.up - followTarget.position, Vector3.up).eulerAngles;
        float rotationAroundX = Mathf.SmoothDampAngle(followTarget.eulerAngles.x, targetDirection.x, ref eulerVelocityX, lockOnSmoothing);
        float rotationAroundY = Mathf.SmoothDampAngle(followTarget.eulerAngles.y, targetDirection.y, ref eulerVelocityY, lockOnSmoothing);

        followTarget.rotation = Quaternion.Euler(rotationAroundX, rotationAroundY, 0);
    }

}
