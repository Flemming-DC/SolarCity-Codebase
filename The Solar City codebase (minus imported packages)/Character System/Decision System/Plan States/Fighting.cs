using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighting : PlanState
{
    [SerializeField] float expectedAttackPauseDuration = 1; 
    [SerializeField] float rotationSpeed = 200f;
    [SerializeField] float faceTargetAngleThreshold = 1f;
    [SerializeField] Transform modelTransform;
    [SerializeField] bool facePlayerBetweenAttacks;
    [SerializeField] List<AttackComboClass> attackCombos;
    [SerializeField] float[] relativeProbabilities;
    [Header("Gizmo")]
    public Attack gizmoAttack;

    public float maxAttackRange { get; private set; } = 1f;
    float minAttackRange = 0f;
    float attackRangeSafetyMargin = 0.3f;
    float targetCapsuleRadius;
    float capsuleRadius;
    float nextAttackTime;
    float minAttackPauseDuration;
    float maxAttackPauseDuration;
    bool readyForAttack;
    bool canBlock;
    List<State> attackStates;
    Func<Vector2> GetBackWards = () => new Vector2(0, -1);
    CharacterDamagable target;
    ActMachine actMachine;
    PlanMachine planMachine;
    ProbabilityDistribution probabilityDistribution;
    Blocking blocking;
    Quaternion currentRotation;

    private void Start()
    {
        blocking = this.GetComponentInSiblings<Blocking>(false);
        canBlock = (blocking != null);
        actMachine = this.GetComponentInSiblings<ActMachine>();
        planMachine = this.GetComponentInSiblings<PlanMachine>();
        
        if (attackCombos.Count != relativeProbabilities.Length)
            Debug.LogWarning($"there must be equally many attacks and relative probabilities");
        probabilityDistribution = new ProbabilityDistribution(relativeProbabilities);
        capsuleRadius = transform.parent.GetComponent<CapsuleCollider>(true).radius;
        SetupRange();
        minAttackPauseDuration = 0.67f * expectedAttackPauseDuration;
        maxAttackPauseDuration = 2 * minAttackPauseDuration;
        attackStates = attackCombos
            .Where(c => c.specialAttack != "")
            .Select(c => actMachine.GetState(c.specialAttack))
            .ToList();
        attackStates.Add(actMachine.GetState<AttackState>());


        if (canBlock)
            blocking.onShieldDestroyed += OnShieldDestroyed;
        actMachine.statemachine.AfterStateChanged += UpdateNextAttackTime;
    }

    private void OnDestroy()
    {
        if (target != null)
            target.OnDie -= ReturnToInitialState;
        if (canBlock)
            blocking.onShieldDestroyed -= OnShieldDestroyed;
        actMachine.statemachine.AfterStateChanged -= UpdateNextAttackTime;
    }

    public override void OnEnter(object target_)
    {
        if (target != null)
            target.OnDie -= ReturnToInitialState;
        
        target = (CharacterDamagable)target_;
        targetCapsuleRadius = GetRadius(target);
        if (canBlock)
            canBlock = blocking.TryEnter();
        if (!canBlock)
            actMachine.SetState<Idle>();
        target.OnDie += ReturnToInitialState;
        nextAttackTime = Time.time;
        currentRotation = modelTransform.rotation;
    }

    public override void Tick()
    {
        if (actMachine.UnInterruptable())
            return;
        if (actMachine.InState<AttackState>())
            return;

        HandleAttackRange(out bool outsideRangeInterval);
        if (outsideRangeInterval)
            return;
        if (!readyForAttack)
            readyForAttack = Time.time >= nextAttackTime;

        if (canBlock)
            canBlock = blocking.TryEnter();
        HandleRotation(out bool facingTarget);
        if (readyForAttack && facingTarget)
            StartCoroutine(HandleAttack());
    }

    public override void OnExit()
    {
        target.OnDie -= ReturnToInitialState;
    }


    void HandleAttackRange(out bool outsideRangeInterval)
    {
        if (InRange(minAttackRange + targetCapsuleRadius + capsuleRadius))
        {
            if (actMachine.InState<Idle>() || actMachine.InState<Blocking>())
                actMachine.SetNextState(typeof(DirectedStrafing), null, (target.transform, GetBackWards));
            outsideRangeInterval = true;
        }
        else if (!InRange(maxAttackRange + targetCapsuleRadius + capsuleRadius))
        {
            planMachine.SetNextState(planMachine.maneuverState, null, target.transform.parent);
            outsideRangeInterval = true;
        }
        else
            outsideRangeInterval = false;
    }

    void HandleRotation(out bool facingTarget)
    {
        Quaternion targetRotation = Quaternion.LookRotation(Displacement().With(y: 0), Vector3.up);
        facingTarget = faceTargetAngleThreshold > Quaternion.Angle(targetRotation, modelTransform.rotation);

        bool isIdle = actMachine.InState<Idle>();
        bool isBlocking = actMachine.InState<Blocking>();
        bool makeRotation = (isIdle || isBlocking) && (readyForAttack || facePlayerBetweenAttacks);
        
        if (makeRotation)
            currentRotation = Quaternion.RotateTowards(
                currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        modelTransform.rotation = currentRotation;
    }

    IEnumerator HandleAttack()
    {
        readyForAttack = false;
        AttackComboClass combo = attackCombos[probabilityDistribution.Draw()];
        if (combo.specialAttack != "")
        {
            State specialAttack = actMachine.GetState(combo.specialAttack);
            actMachine.SetNextState(specialAttack, null, target.transform);
            yield return null;
            yield return new WaitWhile(() => actMachine.statemachine.currentState == specialAttack);
        }
        if (combo.attackCombo.Count != 0)
        {
            actMachine.GetState<AttackState>().target = target.transform;
            actMachine.SetNextState(typeof(AttackState), null, combo.attackCombo);
        }
        yield return null;
    }

    public bool InMaxRangeWithSafetyMargin(Transform target_)
    {
        return (target_.position - transform.position).sqrMagnitude <= Mathf.Pow(maxAttackRange - attackRangeSafetyMargin, 2);
    }

    float GetRadius(CharacterDamagable target_)
    {
        if (target_.transform.parent != null)
        {
            if (target_.transform.parent.TryGetComponent(out CapsuleCollider capsule))
                return capsule.radius;
            else if (target_.transform.parent.TryGetComponent(out Collider collider))
                return collider.bounds.size.With(y: 0).magnitude;
        }
        else if (target_.TryGetComponent(out Collider collider))
            return collider.bounds.size.With(y: 0).magnitude;
        
        Debug.LogWarning($"target {target_} has no collider. Returning {0.4f} by default.");
        return 0.4f;
    }

    void SetupRange()
    {
        maxAttackRange = this.GetComponentInSiblings<WeaponHandler>()
            .weapons.Values.Select(w => w.range).Max();
        minAttackRange = 0;

        float attackRangeSafetyFactor = 1.1f;
        maxAttackRange /= attackRangeSafetyFactor;
        minAttackRange *= attackRangeSafetyFactor;

        if (minAttackRange > maxAttackRange - attackRangeSafetyMargin)
        {
            Debug.LogWarning($"minAttackRange should be less than maxAttackRange - attackRangeSafetyMargin");
            minAttackRange = MathF.Max(0, maxAttackRange - attackRangeSafetyMargin);
        }
    }

    void OnShieldDestroyed()
    {
        canBlock = false;
        blocking.onShieldDestroyed -= OnShieldDestroyed;
        if (actMachine.InState<Blocking>())
            actMachine.SetState<Idle>();
    }

    void UpdateNextAttackTime(State oldState, State newState)
    {
        if (attackStates.Contains(oldState))
            nextAttackTime = Time.time + UnityEngine.Random.Range(minAttackPauseDuration, maxAttackPauseDuration);
    }

    void ReturnToInitialState() => planMachine.SetNextState(planMachine.initialState);
    bool InRange(float range) => Displacement().sqrMagnitude <= range * range;
    Vector3 Displacement() => target.transform.position - transform.position;
    

    [Serializable]
    class AttackComboClass
    {
        public string specialAttack;
        public List<Attack> attackCombo;
    }

    private void OnDrawGizmosSelected()
    {
        if (gizmoAttack == null)
            return;

        // nb: there is no gizmo for left attack range or for max/min attack range
        // float rightAttackRange = this.GetComponentInSiblings<BodyParts>().rightWeapon.GetComponent<Weapon>().range;

        var bodyParts = this.GetComponentInSiblings<BodyParts>();
        float rightRange = 0;
        if (bodyParts.rightWeapon != null)
            if (bodyParts.rightWeapon != null)
                if (bodyParts.rightWeapon.TryGetComponent(out Weapon rightWeapon))
                    rightRange = rightWeapon.range;
        float leftRange = 0;
        if (bodyParts.leftWeapon != null)
            if (bodyParts.leftWeapon != null)
                if (bodyParts.leftWeapon.TryGetComponent(out Weapon leftWeapon))
                    leftRange = leftWeapon.range;
        float maxRange = MathF.Max(leftRange, rightRange);

        Gizmos.color = Color.red;
        Attack attack = gizmoAttack;
        Vector3 forwardDirection = transform.forward;
        Vector3 startPosition = transform.position + attack.sweepHeight * Vector3.up;
        float currentAngle = 0.5f * attack.sweepSizeAngle + attack.sweepAngleOffset;
        float deltaAngle = attack.sweepSizeAngle / (attack.hitDetectionRayCount - 1);
        
        for (int i = 0; i < attack.hitDetectionRayCount; i++)
        {
            Vector3 hitDirection = Quaternion.AngleAxis(attack.sweepOrientationAngle, forwardDirection)
                * Quaternion.Euler(0, currentAngle, 0)
                * forwardDirection;
            Vector3 endPosition = startPosition + hitDirection * maxRange;
            Gizmos.DrawLine(startPosition, endPosition);
            currentAngle -= deltaAngle;
        }

    }



}
