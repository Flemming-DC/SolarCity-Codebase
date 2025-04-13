using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Maneuvering : ManeuverState
{
    [SerializeField] List<string> maneuverNames; 
    [SerializeField] float[] relativeProbabilities;
    [SerializeField] float[] expectedDurations;

    Transform target;
    ActMachine actMachine;
    PlanMachine planMachine;
    Fighting fight;
    List<State> maneuvers;
    ProbabilityDistribution probabilityDistribution;
    float[] GoToNextManeuverProbabilities;
    int index;

    private void Start()
    {
        actMachine = this.GetComponentInSiblings<ActMachine>();
        planMachine = this.GetComponentInSiblings<PlanMachine>();
        fight = this.GetComponentInSiblings<Fighting>();

        if (maneuverNames.Count != relativeProbabilities.Length)
            Debug.LogWarning($"there must be equally many maneuvers and relative probabilities");
        if (maneuverNames.Count != expectedDurations.Length)
            Debug.LogWarning($"there must be equally many maneuvers and expected durations");

        maneuvers = new List<State>();
        foreach (var name in maneuverNames)
            maneuvers.Add(actMachine.GetState(name));
        InitializeGoToNextManeuverProbabilities();
        probabilityDistribution = new ProbabilityDistribution(relativeProbabilities);
    }


    public override void OnEnter(object input)
    {
        target = (Transform)input;
        GoToNextManeuver();
    }

    public override void Tick()
    {
        if (actMachine.UnInterruptable())
            return;
        if (SwitchPlanState())
            return;
        if (InvoluntarilyIdle())
            GoToNextManeuver();

        if (Random.value < Time.deltaTime * GoToNextManeuverProbabilities[index])
            GoToNextManeuver();
    }



    bool SwitchPlanState()
    {
        if (target == null)
        {
            planMachine.SetNextState(planMachine.initialState);
            return true;
        }
        if (fight.InMaxRangeWithSafetyMargin(target))
        {
            if (target.TryGetComponent(out CharacterDamagable damagable))
                planMachine.SetNextState(fight, null, damagable);
            else
                planMachine.SetNextState(fight, null, target.GetComponentInDirectChildren<CharacterDamagable>());
            return true;
        }
        return false;
    }

    void GoToNextManeuver()
    {
        index = probabilityDistribution.Draw();
        actMachine.SetNextState(maneuvers[index], null, target);
    }

    void InitializeGoToNextManeuverProbabilities()
    {
        GoToNextManeuverProbabilities = new float[expectedDurations.Length];
        float GoToTheSameManeuverProbability;
        float sum = relativeProbabilities.Sum();

        for (int i = 0; i < expectedDurations.Length; i++)
        {
            GoToTheSameManeuverProbability = 1 - relativeProbabilities[i] / sum;
            GoToNextManeuverProbabilities[i] = 1 / expectedDurations[i] / GoToTheSameManeuverProbability;
        }
    }


    bool InvoluntarilyIdle() => actMachine.currentStateType == typeof(Idle) && maneuvers[index].GetType() != typeof(Idle);
    //bool CurrentStateIsInvoluntary() => !maneuvers.Contains(actMachine.statemachine.currentState);



}
