using UnityEngine;
using UnityEngine.InputSystem;
using Animancer;

public class AimToThrow : ActionState, IAimThrow
{
    [SerializeField] float angleOffSet = 81;
    [SerializeField] ClipTransition aimingAnimation;
    [SerializeField] GameObject aimingArrowsPrefab;
    [SerializeField] GameObject bombPrefab;
    [SerializeField] Transform model;
    [SerializeField] ALockOnRotator lockOnRotator;

    ActMachine actMachine;
    PlayerMotion playerMotion;
    AnimancerComponent animancer;
    ScivoloMover mover;
    TwoHandingIK twoHandingIK;
    GameObject bombInstance;
    GameObject aimingArrowsInstance;
    AWeaponEquiper equipper;
    Vector3 initialForward;
    bool userGaveInput;
    Quaternion offSet;

    void Start()
    {
        playerMotion = GetComponent<PlayerMotion>();
        actMachine = this.GetComponentInSiblings<ActMachine>();
        animancer = this.GetComponentInSiblings<AnimancerComponent>();
        mover = this.GetComponentInSiblings<ScivoloMover>();
        twoHandingIK = this.GetComponentInSiblings<TwoHandingIK>();
        equipper = this.GetComponentInSiblings<WeaponHandler>().equipper;
        offSet = Quaternion.Euler(0, angleOffSet, 0);
    }


    public override void OnEnter(object dummy)
    {
        twoHandingIK.activationOverriders.Add(this);
        aimingArrowsInstance = Instantiate(aimingArrowsPrefab, model);
        aimingArrowsInstance.transform.Rotate(new Vector3(0, 0, angleOffSet)); // global y = local z
        bombInstance = equipper.ReplaceWeapon(Hand.right, bombPrefab);
        bombInstance.GetComponent<Rigidbody>().useGravity = false;
        bombInstance.GetComponent<Collider>().enabled = false;
        animancer.Play(aimingAnimation, 0.25f);
        mover.velocity = Vector3.zero;
        initialForward = model.forward;
        userGaveInput = false;

    }

    public override void Tick()
    {
        Vector2 rotationInput = InputManager.gameplay.Walk.ReadValue<Vector2>();
        if (!userGaveInput && !lockOnRotator.lockOn)
            userGaveInput = rotationInput != Vector2.zero;

        playerMotion.RotateTowards(RotationDirection(rotationInput));
        if (ChooseDirection())
            actMachine.SetNextState(typeof(Throw), null, bombInstance);
    }

    public override void OnExit()
    {
        twoHandingIK.activationOverriders.Remove(this);
        Destroy(aimingArrowsInstance);
    }


    Vector3 RotationDirection(Vector2 rotationInput)
    {
        if (lockOnRotator.lockOn)
            return offSet * (lockOnRotator.target.position - transform.position).normalized;
        else if (userGaveInput)
            return offSet * playerMotion.GetMoveDirection(rotationInput);
        else
            return offSet * initialForward;
    }

    bool ChooseDirection()
    {
        // check virtually any button on gamepad (not including joystick). 
        if (InputManager.gameplay.Interact.WasPressedThisFrame())
            return true;
        if (InputManager.always.ToggleItemUsage.WasPressedThisFrame())
            return true;
        if (InputManager.gameplay.Jump.WasPressedThisFrame())
            return true;
        if (InputManager.gameplay.Dodge.WasPressedThisFrame())
            return true;
        if (InputManager.gameplay.RightHand1.WasPressedThisFrame())
            return true;
        if (InputManager.gameplay.RightHand2.WasPressedThisFrame())
            return true;
        if (InputManager.gameplay.LeftHand1.WasPressedThisFrame())
            return true;
        if (InputManager.gameplay.LeftHand2.WasPressedThisFrame())
            return true;

        // check anyKey in keyboard
        if (Keyboard.current.anyKey.wasPressedThisFrame)
            return true;

        return false;
    }



    public bool TryThrow(GameObject bombPrefab_)
    {
        if (actMachine.UnInterruptable())
            return false;

        bombPrefab = bombPrefab_;
        actMachine.SetNextState(this);
        return true;
    }




}
