using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomWalk : PlanState
{
    [SerializeField] float maxDistanceFromOriginalPosition = 1000f;
    [SerializeField] float maxDistancePerWalk = 50;
    [SerializeField] float minDistancePerWalk = 10;
    [SerializeField] float waitDuration = 1f;
    [SerializeField] int searchDepth = 10;

    float timer;
    Vector3 originalPosition;
    ActMachine actMachine;

    void Start()
    {
        actMachine = this.GetComponentInSiblings<ActMachine>();
        originalPosition = transform.position;

        if (maxDistancePerWalk < minDistancePerWalk)
            Debug.LogError($"maxDistancePerWalk must be larger than or equal to minDistancePerWalk.");
        if (maxDistanceFromOriginalPosition < maxDistancePerWalk)
            Debug.LogError($"maxDistanceFromOriginalPosition must be larger than or equal to maxDistancePerWalk.");
    }


    public override void OnEnter(object dummy)
    {
        SetNextDestination();
    }

    public override void Tick()
    {
        if (actMachine.UnInterruptable())
            return;

        timer += Time.deltaTime;

        if (actMachine.currentStateType == typeof(AIMotion))
            timer = 0;
        else if (timer > waitDuration)
            SetNextDestination();
    }



    void SetNextDestination()
    {
        timer = 0;
        actMachine.SetNextState(typeof(AIMotion), null, transform.position + LocalRandomPoint());
    }

    Vector3 LocalRandomPoint()
    {
        for (int i = 0; i < searchDepth; i++)
        {
            float angle = Random.Range(0, 2 * Mathf.PI);
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Vector3 localRandomPoint = direction * Random.Range(minDistancePerWalk, maxDistancePerWalk);

            if ((localRandomPoint - originalPosition).sqrMagnitude > maxDistanceFromOriginalPosition * maxDistanceFromOriginalPosition)
                localRandomPoint = -localRandomPoint;
            if (NavMesh.SamplePosition(localRandomPoint, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
                return hit.position;
        }

        Debug.LogWarning($"Failed to find a random point for the new destination.");
        return Vector3.zero;
    }



}
