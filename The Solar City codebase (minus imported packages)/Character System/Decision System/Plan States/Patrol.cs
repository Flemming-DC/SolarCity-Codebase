using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : PlanState
{
    [SerializeField] float waitDuration = 1f;
    [SerializeField] Transform destinationContainer;
    
    float timer;
    int currentIndex;
    ActMachine actMachine;
    Transform currentDestination;
    Transform[] destinations;
    Dictionary<Transform, int> indices = new Dictionary<Transform, int>();

    private void Awake()
    {
        actMachine = this.GetComponentInSiblings<ActMachine>();
        if (destinationContainer != null)
            SetupDestinations();
    }


    public override void OnEnter(object dummy)
    {
        currentDestination = GetNearestDestination();
        actMachine.SetNextState(typeof(AIMotion), null, currentDestination.position);
    }

    public override void Tick()
    {
        if (actMachine.UnInterruptable())
            return;

        timer += Time.deltaTime;

        if (actMachine.currentStateType == typeof(AIMotion))
            timer = 0;
        else if (timer > waitDuration)
            GoToNextDestination();
    }

    public override void OnExit() { }


    void SetupDestinations()
    {
        destinations = destinationContainer.GetComponentsInDirectChildren<Transform>().ToArray();
        if (destinations.Length == 0)
            Debug.LogWarning($"There are no destinations attached to {this}");
        indices = destinations.GetInverse();
    }

    Transform GetNearestDestination()
    {
        if (destinations == null)
            print($"{transform.parent.name}.destinations = null");
        Transform nearest = destinations[0];
        float minSqrDistance = (float)10E+8;
        float sqrDistance;
        foreach(var destination in destinations)
        {
            sqrDistance = (destination.position - transform.position).sqrMagnitude;
            if (sqrDistance < minSqrDistance)
            {
                minSqrDistance = sqrDistance;
                nearest = destination;
            }
        }
        return nearest;
    }


    void GoToNextDestination()
    {
        currentIndex = indices[currentDestination] + 1;
        if (currentIndex >= destinations.Length)
            currentIndex = 0;
        currentDestination = destinations[currentIndex];
        actMachine.SetNextState(typeof(AIMotion), null, currentDestination.position);
    }
}
