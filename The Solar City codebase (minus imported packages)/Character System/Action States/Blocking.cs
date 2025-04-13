using System;
using UnityEngine;
using Animancer;
using ConditionalField;

public class Blocking : ActionState
{
    [SerializeField] BlockAnimations blockAnimations;
    [SerializeField] Transform model;
    [SerializeField] bool isPlayer;
    [ConditionalField(nameof(isPlayer))] [SerializeField] float rotationSpeed = 700;

    public Vector3 rotationDirection { private get; set; }
    public event Action onShieldDestroyed;
    ScivoloMover mover;
    ActMachine actMachine;
    AnimancerComponent animancer;
    PlayerMotion playerMotion;
    AnimancerState blockStartState;
    WeaponHandler weaponHandler;

    private void Start()
    {
        mover = this.GetComponentInSiblings<ScivoloMover>();
        actMachine = this.GetComponentInSiblings<ActMachine>();
        animancer = this.GetComponentInSiblings<AnimancerComponent>();
        weaponHandler = this.GetComponentInSiblings<WeaponHandler>();
        interruptable = true;
        if (isPlayer)
            playerMotion = this.GetComponentInSiblings<PlayerMotion>();
    }

    public bool TryEnter()
    {
        if (!TryGetShieldDamagable(out PropsDamagable dummy))
            return false;

        actMachine.SetNextState(this);
        return true;
    }

    public override void OnEnter(object dummy1)
    {
        if (!TryGetShieldDamagable(out PropsDamagable dummy2))
            actMachine.SetNextState(typeof(Idle));

        mover.velocity = Vector3.zero;
        blockStartState = animancer.Play(blockAnimations.blockStart, 0.25f, FadeMode.FixedDuration);
    }

    public override void Tick()
    {
        if (isPlayer)
        {
            Vector2 rotationInput = InputManager.gameplay.Walk.ReadValue<Vector2>();
            rotationDirection = playerMotion.GetMoveDirection(rotationInput);
        }
        RotateTowards(rotationDirection);

        if (blockStartState != null)
            if (blockStartState.NormalizedTime >= blockStartState.NormalizedEndTime)
                animancer.Play(blockAnimations.blockLoop, 0.25f, FadeMode.FixedDuration);
    }

    public bool TryBlock(Weapon weapon, RaycastHit hit)
    {
        float angle = Vector3.Angle(model.forward, weapon.owner.transform.position - transform.position);
        if (angle > CombatSettings.instance.maxBlockingAngle)
            return false;
        bool blocked = actMachine.statemachine.currentState == this && weapon.attack.blockable;
        if (!blocked)
        {
            if (!weapon.attack.blockable)
                DealDamageToShield(weapon); 
            return false;
        }

        weapon.Recoil(hit, true);
        animancer.Play(blockAnimations.blockHit)
            .Events.OnEnd = () => animancer.Play(blockAnimations.blockLoop);
        return true;
    }

    void DealDamageToShield(Weapon weapon)
    {
        if (TryGetShieldDamagable(out PropsDamagable shieldDamagable))
        {
            RaycastHit dummyHit = new RaycastHit();
            shieldDamagable.TakeDamage(weapon, dummyHit);
            if (shieldDamagable.health <= 0)
                onShieldDestroyed?.Invoke();
        }
    }


    bool TryGetShieldDamagable(out PropsDamagable shieldDamagable)
    {
        if (weaponHandler.weapons[Hand.left] != null)
        {
            if (weaponHandler.weapons[Hand.left].TryGetComponent(out shieldDamagable))
                return true;
        }
        else if (weaponHandler.weapons[Hand.left] != null)
        {
            if (weaponHandler.weapons[Hand.right].TryGetComponent(out shieldDamagable))
                return true;
        }
        
        shieldDamagable = null; 
        return false;
    }



    void RotateTowards(Vector3 movementDirection)
    {
        if (movementDirection == Vector3.zero)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(movementDirection, transform.up);
        model.rotation = Quaternion.RotateTowards(model.rotation,
                                                  targetRotation,
                                                  rotationSpeed * Time.deltaTime);
    }



}
