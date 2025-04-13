using System;
using System.Collections;
using UnityEngine;

public class IdlePlan : PlanState
{
    [SerializeField] bool returnToInitialPosition;

    ActMachine actMachine;
    Vector3 initialPosition;
    Quaternion initialRotation;

    private void Awake()
    {
        actMachine = this.GetComponentInSiblings<ActMachine>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }


    public override void OnEnter(object dummy)
    {
        StartCoroutine(ReturnToStartingPosition());
    }

    IEnumerator ReturnToStartingPosition()
    {
        if (Time.timeSinceLevelLoad < Time.deltaTime)
            yield return null;
        yield return new WaitUntil(() => !actMachine.UnInterruptable());

        if (returnToInitialPosition && transform.position != initialPosition)
            actMachine.SetNextState(typeof(AIMotion), null, initialPosition);
        else
            actMachine.SetNextState(typeof(Idle));

        yield return new WaitUntil(() => actMachine.InState<Idle>());
        Func<Quaternion> GetRotation = () => initialRotation;
        actMachine.SetState<Rotate>(GetRotation);
    }


}
