using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : RootMotionAction, IAttackState
{
    [SerializeField] Transform modelTransform;
    public Attack fallingAttack;
    [SerializeField] bool autoContinueDuringCombos = true;
    [SerializeField] bool loopingDuringCombo = false;
    [SerializeField] float rotationSpeed = 100;
    public float attackSpeedMultiplier = 1;

    public Stat attackDamageModifier { get; private set; } = 1;
    public Stat attackSpeedModifier { get; private set; } = 1;
    public Stat bloodScaleModifier { get; private set; } = 1;
    public Attack attack { get; private set; }
    public bool swingIsFinishedWithDelay { get; set; }
    public AttackProggresion attackProggresion {get; set;}
    public Hand hand { get; private set; }
    public AttackType attackType { get; private set; }
    public Transform target { private get; set; } // the Character will try to face the target, if the attackData says so. 
    List<Attack> attackCombo = new List<Attack>();
    TwoHandingIK twoHandingIK;
    WeaponHandler weaponHandler;
    int attackComboIndex;
    bool hasLandingState;
    bool isPlayer;

    protected override void Start()
    {
        base.Start();
        weaponHandler = this.GetComponentInSiblings<WeaponHandler>();
        twoHandingIK = this.GetComponentInSiblings<TwoHandingIK>();
        hasLandingState = actMachine.states.ContainsKey(typeof(Landing));
        attackProggresion = AttackProggresion.notAttacking;
        isPlayer = transform.parent.TryGetComponent(out PlayerReferencer _);
        maxTime = 10;
    }

    public override void OnEnter(object input)
    {
        SetupAttack(input);
        PerformAttack(attack);
    }

    public override void Tick()
    {
        interruptable = swingIsFinishedWithDelay;
        HandleRotation();
        if (autoContinueDuringCombos)
            TryPerformNextAttack();
        base.Tick();
    }

    public override void OnExit()
    {
        base.OnExit();
        attackProggresion = AttackProggresion.notAttacking;
        twoHandingIK.activationOverriders.Remove(this);
    }

    void SetupAttack(object input)
    {
        attackComboIndex = 0;
        
        if (!mover.isGrounded)
        {
            attackCombo = new List<Attack>();
            attack = fallingAttack;
        }
        else if (input.GetType() == typeof(Attack))
        {
            attackCombo = new List<Attack>();
            attack = (Attack)input;
        }
        else if (input.GetType() == typeof(List<Attack>))
        {
            attackCombo = new List<Attack>((List<Attack>)input);
            if (attackCombo == null)
                attackCombo = new List<Attack>();
            attack = attackCombo[0];
        }
        else if (input.GetType() == typeof((Hand, AttackType)))
        {
            (hand, attackType) = ((Hand, AttackType))input;
            (attack, attackCombo) = weaponHandler.weapons[hand].GetAttack(hand, attackType);
        }

    }

    public void TryPerformNextAttack()
    {
        if (swingIsFinishedWithDelay)
            if (TryGetNextAttack(out Attack nextAttack))
                PerformAttack(nextAttack);
    }

    void PerformAttack(Attack attack_)
    {
        if (attack_.isBlocking)
        {
            bool success = actMachine.GetState<Blocking>().TryEnter();
            if (!success)
                Debug.LogWarning($"Failed to enter blocking on {transform.parent.Path()}");
            return;
        }
        if (isPlayer && !Stamina.TryPayStaminaCost(attack.staminaCost))
        {
            actMachine.SetState<Idle>();
            return;
        }

        attack = attack_;
        if (!isPlayer)
            hand = attack_.hand;
        SetModifiers(attack);
        weaponHandler.weapons[hand].bloodScale.SetFactor(bloodScaleModifier);
        weaponHandler.weapons[hand].attack = attack_;
        weaponHandler.weapons[hand].forwardDirection = modelTransform.forward;
        attackProggresion = AttackProggresion.windup;
        if (attack.forceOneHandedness)
            twoHandingIK.activationOverriders.Add(this);
        actionAnimation = attack_.animation;
        swingIsFinishedWithDelay = false;
        base.OnEnter(null);
        animancer.States.Current.Speed = attackSpeedMultiplier * attack_.windUpSpeed;
    }

    bool TryGetNextAttack(out Attack nextAttack)
    {
        attackComboIndex++;
        if (attackComboIndex == attackCombo?.Count)
            if (loopingDuringCombo)
                attackComboIndex = 0;

        if (attackComboIndex < attackCombo?.Count)
        {
            nextAttack = attackCombo[attackComboIndex];
            return true;
        }
        else
        {
            nextAttack = null;
            return false;
        }
    }
    
    protected override void SwitchStateIfNeeded()
    {
        if (attack != fallingAttack)
            base.SwitchStateIfNeeded();
        else if (mover.isGroundedWithLag && animancer.IsFinished(0.5f))
        {
            if (hasLandingState)
                actMachine.SetNextState(typeof(Landing));
            else
                actMachine.SetNextState(typeof(Idle));
            DealDamageFromFallingAttack();
        }
    }


    void DealDamageFromFallingAttack()
    {
        float range = weaponHandler.weapons[hand].range;
        Vector3 hitDetectionCenter = transform.position + 0.5f * range * modelTransform.forward + weaponHandler.weapons[hand].attack.sweepHeight * Vector3.up;
        Collider[] targets = Physics.OverlapSphere(hitDetectionCenter, range);
        foreach (var target in targets)
            if (target.TryGetComponentInCharacter(out CharacterDamagable damagable))
                if (target.transform != weaponHandler.weapons[hand].owner)
                    weaponHandler.weapons[hand].DealDamage(target.transform, damagable, out bool dummy);
    }

    
    void HandleRotation()
    {
        if (target == null)
            return;
        if (attack == null)
            return;
        if (!attack.canRotateDuringWindUp)
            return;
        if (attackProggresion != AttackProggresion.windup)
            return;
        if (isPlayer)
            return;

        Vector3 displacement = (target.transform.position - transform.position).With(y: 0);
        Quaternion targetRotation = Quaternion.LookRotation(displacement, Vector3.up);
        modelTransform.rotation = Quaternion.RotateTowards(
            modelTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }


    void SetModifiers(Attack attack)
    {
        attack.damage.SetFactor(attackDamageModifier);
        attack.windUpSpeed.SetFactor(attackSpeedModifier);
        attack.swingSpeed.SetFactor(attackSpeedModifier);
        attack.rewindSpeed.SetFactor(attackSpeedModifier);
    }

}

public enum AttackType { firstAttack, secondAttack };
public enum AttackProggresion { windup, swing, rewind, notAttacking}