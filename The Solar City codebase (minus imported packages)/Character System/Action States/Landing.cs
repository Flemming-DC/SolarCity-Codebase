using System;
using UnityEngine;
using Animancer;

public class Landing : ActionState
{
    [SerializeField] float HardLandingThreshold = 4;
    [SerializeField] float killDistance = 11;
    [SerializeField] float fallDamageExponent = 2.5f;
    [SerializeField] ClipTransition landingAnimation;
    [SerializeField] ClipTransition hardLandingAnimation;

    Transform characterTransform;
    AnimancerComponent animancer;
    ScivoloMover mover;
    Falling falling;
    AttackState attackState;
    CharacterDamagable damagable;
    Sound hurtSound;
    bool noLandingAnimation;
    float fallHeight;
    float fallDistance;

    private void Start()
    {
        characterTransform = transform.parent;
        ActMachine actMachine = this.GetComponentInSiblings<ActMachine>();
        animancer = this.GetComponentInSiblings<AnimancerComponent>();
        mover = this.GetComponentInSiblings<ScivoloMover>();
        falling = this.GetComponent<Falling>(true);
        attackState = this.GetComponent<AttackState>();
        damagable = this.GetComponentInSiblings<CharacterDamagable>();
        hurtSound = this.GetComponent<Hurt>(true).hurtSound;
        CombatSettings.instance.softLandingSound?.MakeSource(gameObject);
        fallHeight = transform.position.y;
        inVoluntary = true;
        maxTime = 10;

        Func<bool> LandingIsFinished = () => noLandingAnimation || animancer.IsFinished();
        actMachine.AddTransition(this, typeof(Idle), LandingIsFinished);

        mover.OnGroundedChanged += OnGroundedChanged;
    }

    void OnDestroy() => mover.OnGroundedChanged -= OnGroundedChanged;


    public override void OnEnter(object dummy)
    {
        mover.velocity = Vector3.zero;
        falling.canFall = false;
        noLandingAnimation = false;
        if (Time.time > 0.1f)
            CombatSettings.instance.softLandingSound?.Play(gameObject);

        if (fallDistance < HardLandingThreshold && landingAnimation.Clip == null)
            noLandingAnimation = true;
        else if (fallDistance < HardLandingThreshold && landingAnimation.Clip != null)
            animancer.Play(landingAnimation, 0.15f, FadeMode.FixedDuration);
        else
        {
            hurtSound?.Play(gameObject);
            TakeFallingDamage(fallDistance);
            if (attackState != null)
                if (attackState.attack == attackState.fallingAttack)
                    return;
            animancer.Play(hardLandingAnimation, 0.15f, FadeMode.FixedDuration);
        }
    }

    public override void Tick()
    {
        mover.velocity = animancer.Animator.deltaPosition / Time.deltaTime;
        characterTransform.rotation *= animancer.Animator.deltaRotation;
    }

    public override void OnExit()
    {
        falling.canFall = true;
        falling.ResetTwoHanding();
    }


    void OnGroundedChanged(bool isGroundedWithLag, float height)
    {
        if (isGroundedWithLag && Time.timeSinceLevelLoad > 3 * Time.deltaTime)
            fallDistance = fallHeight - height;
        else
        {
            fallHeight = height;
            fallDistance = 0;
        }
    }

    public void TakeFallingDamage(float fallDistance_)
    {
        if (fallDistance_ < HardLandingThreshold)
            return;
        float damagePct = Mathf.Pow(fallDistance_ / killDistance, fallDamageExponent);
        damagable.TakeDamageDirectly(damagePct * damagable.MaxHealth);
    }

}
