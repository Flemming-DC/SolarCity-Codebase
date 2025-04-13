using System;
using UnityEngine;
using UnityEngine.AI;

public class Chase : AIMotion
{
    public Sound roar;
    
    Transform target;
    UpdateTimer updateTimer = new UpdateTimer();

    protected override void Start()
    {
        updateTimer.Start(GetComponentInParent<CharacterReferencer>(), 0.3f, 1f);
        roar?.MakeSource(gameObject);
        base.Start();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        updateTimer.Stop();
    }

    public override void OnEnter(object target_)
    {
        target = (Transform)target_;
        roar?.Play(gameObject);
        base.OnEnter(target.position);
    }

    public override void Tick()
    {
        if (target == null)
        {
            actMachine.SetNextState(typeof(Idle));
            return;
        }
        if (updateTimer.TimeToUpdate())
        {
            agent.SetDestination(target.position);
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(agent.destination, path);
            if (agent.pathStatus == NavMeshPathStatus.PathComplete)
                agent.SetPath(path);
        }
        base.Tick();
    }




}
