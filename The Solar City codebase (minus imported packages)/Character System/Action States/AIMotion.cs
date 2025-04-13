using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Animancer;

public class AIMotion : ActionState
{
    // You can set the motion parameters on the NavMeshAgent.
    [SerializeField] Transform modelTransform;
    public float rotationSpeed = 700;

    public NavMeshAgent agent { get; private set; }
    protected ActMachine actMachine;
    AnimancerComponent animancer;
    AnimationUpdater animationUpdater;
    ScivoloMover mover;
    ClipTransition move;


    protected virtual void Start()
    {
        agent = GetComponentInParent<NavMeshAgent>();
        actMachine = this.GetComponentInSiblings<ActMachine>();
        animancer = this.GetComponentInSiblings<AnimancerComponent>();
        mover = this.GetComponentInSiblings<ScivoloMover>();
        animationUpdater = this.GetComponentInSiblings<AnimationUpdater>();
        interruptable = true;

        //Func<bool> destinationReached = () => (transform.position - agent.destination).sqrMagnitude < agent.stoppingDistance * agent.stoppingDistance;
        //actMachine.AddTransition(this, typeof(Idle), destinationReached);
        SetAnimation(animationUpdater.animations);
        if (animationUpdater != null)
            animationUpdater.OnAnimationsChanged += SetAnimation;
        agent.enabled = false;
    }

    protected virtual void OnDestroy()
    {
        if (animationUpdater != null)
            animationUpdater.OnAnimationsChanged -= SetAnimation;
    }

    public override void OnEnter(object destination)
    {
        agent.enabled = true;
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning(
                $"the character {transform.parent.Path()} is at position = {transform.position}, " +
                $"which isn't on the NavMesh. This character will be destroyed.");
            Destroy(transform.parent.gameObject);
            return;
        }
        mover.Enabled(false);
        animancer.Play(move, 0.25f, FadeMode.FixedDuration);
        StartCoroutine(RotateTowards());
        agent.SetDestination((Vector3)destination);
        if (agent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            print("bad path");
            actMachine.SetState<Idle>();
        }
    }



    public override void Tick()
    {
        float remainingDistanceSqr = (transform.position - agent.destination).sqrMagnitude;
        if (remainingDistanceSqr < agent.stoppingDistance * agent.stoppingDistance)
            actMachine.SetState<Idle>();
    }

    public override void OnExit()
    {
        agent.enabled = false;
        mover.Enabled(true);
    }
    
    IEnumerator RotateTowards()
    {
        while (Math.Abs(modelTransform.localRotation.eulerAngles.y) > 1f)
        {
            modelTransform.localRotation = Quaternion.RotateTowards(
                modelTransform.localRotation, Quaternion.identity, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        modelTransform.localRotation = Quaternion.identity;
    }
    
    void SetAnimation(AnimationsGivenWeaponClass animations)
    {
        move = animations.run;
        if (actMachine.statemachine.currentState == this)
            animancer.Play(move, 0.25f);
    }


}

