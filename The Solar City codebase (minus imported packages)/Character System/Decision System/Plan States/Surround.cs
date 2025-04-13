using UnityEngine;
using Random = UnityEngine.Random;

public class Surround : ManeuverState
{
    [SerializeField] float expectedDuration = 1;
    [SerializeField] float perpendicularOffset = 3;
    [SerializeField] float angleThreshold = 45;
    [SerializeField] float destinationUpdateDuration = 0.5f;

    Transform target;
    ActMachine actMachine;
    PlanMachine planMachine;
    Fighting fight;
    Transform targetModel;
    Transform model;
    float sign;
    float timeAtLastSetDestination;

    private void Start()
    {
        actMachine = this.GetComponentInSiblings<ActMachine>();
        planMachine = this.GetComponentInSiblings<PlanMachine>();
        model = this.GetComponentInSiblings<BodyParts>().transform;
        fight = this.GetComponentInSiblings<Fighting>();

    }


    public override void OnEnter(object input)
    {
        target = (Transform)input;
        targetModel = target.GetComponentInDirectChildren<BodyParts>().transform;

        float angle = Vector3.SignedAngle(model.forward, -targetModel.forward, Vector3.up);
        sign = angle >= 0 ? 1 : -1;
        UpdateDestination(enter: true);
    }

    public override void Tick()
    {
        if (actMachine.UnInterruptable())
            return;
        if (SwitchPlanState())
            return;
        if (actMachine.InState<Idle>())
            UpdateDestination(enter: true);
        else if (actMachine.InState<AIMotion>() && !actMachine.InState<Chase>())
            UpdateDestination(enter: false);

        if (StartChase())
            actMachine.SetState<Chase>(target);
    }


    
    bool SwitchPlanState()
    {
        // this function is just like the default maneuvering state
        if (target == null)
        {
            planMachine.SetNextState(planMachine.initialState);
            return true;
        }
        if (fight.InMaxRangeWithSafetyMargin(target))
        {
            var damagable = target.GetComponentInCharacter<CharacterDamagable>();
            planMachine.SetNextState(fight, null, damagable);
            return true;
        }
        return false;
    }


    void UpdateDestination(bool enter)
    {
        if (Time.time - timeAtLastSetDestination < destinationUpdateDuration)
            return;
        timeAtLastSetDestination = Time.time;
        Vector3 perpendicularDirection = Quaternion.Euler(0, sign * 90, 0) * (targetModel.position - model.position).normalized;
        Vector3 destination = targetModel.position + perpendicularOffset * perpendicularDirection;

        if (enter)
            actMachine.SetState<AIMotion>(destination);
        else // dette bryder med at kun basis tilstande må udfører arbejdet
            actMachine.GetState<AIMotion>().agent.SetDestination(destination); 
    }

    bool StartChase()
    {
        var direction = targetModel.position - transform.position;
        var angle = Vector3.Angle(direction, targetModel.forward);

        if (angle > angleThreshold)
            return false;
        else if (angle > 5)
            return Random.value < Time.deltaTime / expectedDuration;
        else
            return true;
    }

}
