using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GlobalCharacterStateDisplay : MonoBehaviour
{
    [SerializeField] bool printActState;
    [SerializeField] bool printPlanState;
    [SerializeField] TMP_Text actDebugText;
    [SerializeField] TMP_Text planDebugText;
    [SerializeField] GameObject character;

    ActMachine actMachine;
    PlanMachine planMachine;
    Dictionary<(State, State), int> recentStateChange = new Dictionary<(State, State), int>();

    private void OnEnable()
    {
        if (CharacterIsNullOrDisabled())
            return;
        
        if (!character.TryGetComponent(out actMachine))
            actMachine = character.GetComponentInDirectChildren<ActMachine>();
        if (!character.TryGetComponent(out planMachine))
            planMachine = character.GetComponentInDirectChildren<PlanMachine>(false);

        Invoke(nameof(LateOnEnable), Time.deltaTime);
    }

    private void OnDisable()
    {
        if (CharacterIsNullOrDisabled())
            return;
        actMachine.statemachine.BeforeStateChanged -= DisplayState;
        if (planMachine != null)
            planMachine.statemachine.BeforeStateChanged -= DisplayState;
    }

    void LateOnEnable()
    {
        actMachine.statemachine.BeforeStateChanged += DisplayState;
        if (planMachine != null)
            planMachine.statemachine.BeforeStateChanged += DisplayState;
        
        DisplayState(actMachine.statemachine.currentState, actMachine.statemachine.currentState);
        if (planMachine != null) 
            DisplayState(planMachine.statemachine.currentState, planMachine.statemachine.currentState);
    }


    void DisplayState(State oldState, State newState)
    {
        if (CharacterIsNullOrDisabled())
            return;
        if (newState == null)
            return;
        
        TMP_Text debugText = oldState is ActionState ? actDebugText : planDebugText;
        bool printState = oldState is ActionState ? printActState : printPlanState;
        debugText.text = $"{character.name}: {newState.GetType()}";
        if (printState)
            print(debugText.text);
        StartCoroutine(UpdateRecentStateChange(oldState, newState, oldState is ActionState));
    }

    IEnumerator UpdateRecentStateChange(State oldState, State newState, bool isActMachine)
    {
        if (!recentStateChange.ContainsKey((oldState, newState)))
            recentStateChange.Add((oldState, newState), 0);
        recentStateChange[(oldState, newState)] += 1;

        StatemachineBehavior machine = isActMachine ? (StatemachineBehavior)actMachine : (StatemachineBehavior)planMachine;
        if (recentStateChange[(oldState, newState)] >= 5)
            Debug.LogWarning($"the statemachineBehavior {machine} on {character.name} has transitioned from " +
                             $"{oldState} to {newState} {recentStateChange[(oldState, newState)]} times " +
                             $"in a single second. This is probably an error.");

        yield return new WaitForSeconds(1);
        recentStateChange[(oldState, newState)] = 0;
    }

    bool CharacterIsNullOrDisabled()
    {
        if (character == null)
            return true;
        return !character.activeInHierarchy;

    }

}
