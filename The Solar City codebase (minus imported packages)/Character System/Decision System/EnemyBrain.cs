using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField] float memoryDuration = 5;
    [SerializeField] UpdateTimer updateTimer;

    PlanMachine planMachine;
    TeamHandler teamHandler;
    CharacterDamagable damagable;
    Vision vision;
    ISense[] senses;
    GameObject target;
    float memoryTimer;


    void Start()
    {
        planMachine = this.GetComponentInSiblings<PlanMachine>();
        teamHandler = this.GetComponentInSiblings<TeamHandler>();
        vision = this.GetComponentInSiblings<Vision>();
        damagable = this.GetComponentInSiblings<CharacterDamagable>(true);
        senses = GetComponents<ISense>();

        updateTimer.Start(GetComponentInParent<CharacterReferencer>(), 0.1f, 0.4f);
        damagable.OnHealthChanged += OnHealthChanged;
    }


    void OnDestroy()
    {
        damagable.OnHealthChanged -= OnHealthChanged;
        updateTimer.Stop();
    }


    void Update()
    {
        if (planMachine.InState<Fighting>())
            return;
        if (IsUpdatingMemoryTimer())
            return;
        if (memoryTimer <= 0 && updateTimer.TimeToUpdate())
            UpdateAggresion();

    }

    void UpdateAggresion()
    {
        foreach (var sense in senses)
            foreach (var detectedObject in sense.detectedObjects)
                if (teamHandler.IsHostileTo(detectedObject))
                {
                    Aggro(detectedObject);
                    return;
                }

        bool isManeuvering = planMachine.currentStateType == planMachine.maneuverState.GetType();
        if (isManeuvering || planMachine.InState<Fighting>())
        {
            planMachine.SetNextState(planMachine.initialState);
            target = null;
            return;
        }
    }



    void Aggro(GameObject target_)
    {
        memoryTimer = memoryDuration;
        target = target_;
        planMachine.SetNextState(planMachine.maneuverState, null, target_.transform);
    }

    bool IsUpdatingMemoryTimer()
    {
        if (target == null)
            return false;
        if (memoryTimer <= 0)
            return false;
        memoryTimer -= Time.deltaTime;
        if (vision.InSight(target.transform))
            return false;

        return true;

    }

    // this should evt. go into a separate detection script
    void OnHealthChanged(GameObject attacker, float oldHealth, float newHealth)
    {
        if (planMachine.currentStateType == typeof(Fighting))
            return;
        if (attacker == null)
            return;
        if (!attacker.TryGetComponentInCharacter(out CharacterDamagable _))
            return;
        if (!teamHandler.IsHostileTo(attacker))
            return;

        // evt talk to attacker, if not hostile
        Aggro(attacker);
    }

}
