using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDecider : MonoBehaviour
{
    [SerializeField] ALockOnRotator lockOnRotator;

    ActMachine actMachine;
    PlayerStrafing strafing;
    PlayerMotion playerMotion;
    WeaponHandler weaponHandler;
    Dodge dodge;

    private void Start()
    {
        actMachine = this.GetComponentInSiblings<ActMachine>();
        strafing = this.GetComponentInSiblings<PlayerStrafing>();
        playerMotion = this.GetComponentInSiblings<PlayerMotion>();
        weaponHandler = this.GetComponentInSiblings<WeaponHandler>();
        dodge = this.GetComponentInSiblings<Dodge>();

        actMachine.statemachine.AfterStateChanged += OnStateChanged;
        SetupBaseMotion();
        SetupAttacks();
        InputManager.gameplay.Jump.performed += dummy => Jump();
        InputManager.gameplay.Dodge.performed += dummy => Dodge();
        InputManager.gameplay.LeftHand1.canceled += dummy => StopBlocking();
        InputManager.gameplay.LeftHand2.canceled += dummy => StopBlocking();
    }

    private void OnDestroy()
    {
        actMachine.statemachine.AfterStateChanged -= OnStateChanged;
        InputManager.gameplay.Disable();
    }

    private void Update()
    {
        if (InputManager.gameplay.Walk.ReadValue<Vector2>() != Vector2.zero)
            actMachine.SetNextState(actMachine.motionState.GetType(), typeof(Idle));
        else
            actMachine.SetNextState(typeof(Idle), actMachine.motionState.GetType());


    }
    

    void SetupBaseMotion()
    {
        InputManager.gameplay.Sprint.performed += dummy => actMachine.SwitchMotionState(playerMotion);
        InputManager.gameplay.Sprint.canceled += dummy => SetStrafing();
        InputManager.gameplay.LockOn.performed += dummy => Invoke(nameof(SetStrafing), 2 * Time.deltaTime);
    }

    void SetupAttacks()
    {
        var attackState = actMachine.GetState<AttackState>();
        InputManager.gameplay.RightHand1.performed += dummy =>
        {
            if (actMachine.InState<AttackState>() && attackState.hand == Hand.right && attackState.attackType == AttackType.firstAttack)
                attackState.TryPerformNextAttack();
            else
                Attack(Hand.right, AttackType.firstAttack);
        };
        InputManager.gameplay.LeftHand1.performed += dummy =>
        {
            if (actMachine.InState<AttackState>() && attackState.hand == Hand.left && attackState.attackType == AttackType.firstAttack)
                attackState.TryPerformNextAttack();
            else
                Attack(Hand.left, AttackType.firstAttack);
        };
        InputManager.gameplay.RightHand2.performed += dummy => Attack(Hand.right, AttackType.secondAttack);
        InputManager.gameplay.LeftHand2.performed  += dummy => Attack(Hand.left, AttackType.secondAttack);
    }


    void Jump()
    {
        if (actMachine.UnInterruptable())
            return;

        actMachine.SetNextState(typeof(Jump), typeof(Idle));
        actMachine.SetNextState(typeof(ForwardJump), actMachine.motionState.GetType(), InputManager.gameplay.Walk.ReadValue<Vector2>());
    }

    void Dodge()
    {
        if (actMachine.UnInterruptable())
            return;

        Vector2 inputDirection = InputManager.gameplay.Walk.ReadValue<Vector2>();
        dodge.TryEnter(inputDirection);
    }

    void StopBlocking()
    {
        if (actMachine.UnInterruptable())
            return;
        if (!actMachine.states.ContainsKey(typeof(Blocking)))
            return;

        actMachine.SetNextState(typeof(Idle), typeof(Blocking));
    }

    void Attack(Hand hand, AttackType attackType)
    {
        bool hasFallingAttack = weaponHandler.weapons[Hand.right].hasFallingAttack || weaponHandler.weapons[Hand.left].hasFallingAttack;
        bool makeFallingAttack = actMachine.InState<Falling>() && hasFallingAttack;
        if (actMachine.UnInterruptable() && !makeFallingAttack)
            return;

        actMachine.SetNextState(typeof(AttackState), null, (hand, attackType));
    }

    void OnStateChanged(State oldState, State newState)
    {
        if (newState.GetType() != typeof(Idle))
            return;

        Hand hand;
        AttackType attackType;
        if (InputManager.gameplay.RightHand1.IsPressed())
            (hand, attackType) = (Hand.right, AttackType.firstAttack);
        else if (InputManager.gameplay.RightHand2.IsPressed())
            (hand, attackType) = (Hand.right, AttackType.secondAttack);
        else if (InputManager.gameplay.LeftHand1.IsPressed())
            (hand, attackType) = (Hand.left, AttackType.firstAttack);
        else if (InputManager.gameplay.LeftHand2.IsPressed())
            (hand, attackType) = (Hand.left, AttackType.secondAttack);
        else
            return;

        (Attack attack, _) = weaponHandler.weapons[hand].GetAttack(hand, attackType);
        if (!attack.isBlocking)
            return;

        actMachine.SetNextState(typeof(AttackState), null, attack);
    }

    void SetStrafing()
    {
        if (lockOnRotator.lockOn)
            actMachine.SwitchMotionState(strafing);
        else
            actMachine.SwitchMotionState(playerMotion);
    }




}
