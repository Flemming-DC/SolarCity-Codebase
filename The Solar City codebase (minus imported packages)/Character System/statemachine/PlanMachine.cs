using UnityEngine;

public class PlanMachine : StatemachineBehavior
{
    [SerializeField] string maneuverStateName = "Maneuvering";

    public State maneuverState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        maneuverState = GetState(maneuverStateName);
    }


}

