using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

//#if UNITY_EDITOR
public class DisplayState : MonoBehaviour
{
    [SerializeField] GameObject displayPrefab;
    [SerializeField] float heightMultiplier = 1;

    float height;
    Transform cameraTransform;
    Transform display;
    TMP_Text actStateText;
    TMP_Text planStateText;
    ActMachine actMachine;
    PlanMachine planMachine;
    Dictionary<(State, State), int> recentStateChange = new Dictionary<(State, State), int>();

    void Start()
    {
        actMachine = GetComponent<ActMachine>();
        planMachine = this.GetComponentInSiblings<PlanMachine>(false);
        CapsuleCollider collider = GetComponentInParent<CapsuleCollider>();
        cameraTransform = Camera.main.transform;

        display = Instantiate(displayPrefab, transform).transform;
        actStateText = display.GetChild(0).GetComponent<TMP_Text>();
        planStateText = display.GetChild(1).GetComponent<TMP_Text>();
        if (planMachine == null)
            planStateText.text = "";
        height = collider.height;
        actStateText.enabled = false;
        planStateText.enabled = false;
        /*
        try
        {
            height = collider.height;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"failed to set height on StateDisplay");
            print($"height = {collider}.height");
            print(e.Message);
            print(e.StackTrace);
            print($"time = {Time.timeSinceLevelLoad}");
            print($"path = {transform.GetPath()}");
            height = 2;
        }*/

        actMachine.statemachine.BeforeStateChanged += UpdateState;
        if (planMachine != null)
            planMachine.statemachine.BeforeStateChanged += UpdateState;
        UpdateState(actMachine.statemachine.currentState, actMachine.statemachine.currentState);
        if (planMachine != null)
            UpdateState(planMachine.statemachine.currentState, planMachine.statemachine.currentState);

        InputManager.editor.ShowState.performed += _ => OnShowState();
    }
    private void OnDestroy()
    {
        actMachine.statemachine.BeforeStateChanged -= UpdateState;
        if (planMachine != null)
            planMachine.statemachine.BeforeStateChanged -= UpdateState;

        InputManager.editor.ShowState.performed -= _ => OnShowState();
    }


    void LateUpdate()
    {
        display.localPosition = heightMultiplier * height * Vector3.up;
        display.LookAt(display.position + cameraTransform.rotation * Vector3.forward,
                       cameraTransform.rotation * Vector3.up); // rotate to face player
    }


    void OnShowState()
    {
        if (actStateText == null || planStateText == null)
            return;
        actStateText.enabled = !actStateText.enabled;
        planStateText.enabled = !planStateText.enabled;
    }


    void UpdateState(State oldState, State newState)
    {
        if (newState == null)
            return;

        TMP_Text stateText = oldState is ActionState ? actStateText : planStateText;
        stateText.text = newState.GetType().Name;
        if (enabled && gameObject.activeSelf)
            StartCoroutine(UpdateRecentStateChange(oldState, newState, oldState is ActionState));
    }

    IEnumerator UpdateRecentStateChange(State oldState, State newState, bool isActMachine)
    {
        if (!recentStateChange.ContainsKey((oldState, newState)))
            recentStateChange.Add((oldState, newState), 0);
        recentStateChange[(oldState, newState)] += 1;

        var machine = isActMachine ? actMachine.statemachine : planMachine.statemachine;
        string machineName = isActMachine ? "actMachine" : "planMachine";
        if (recentStateChange[(oldState, newState)] >= 5)
            Debug.LogWarning($"the statemachineBehavior {machineName} on {transform.parent.Path()} has transitioned from " +
                             $"{oldState} to {newState} {recentStateChange[(oldState, newState)]} times " +
                             $"in a single second. This is probably an error.\nnb: {newState}'s input is {machine.currentInput}");

        yield return new WaitForSeconds(1);
        recentStateChange[(oldState, newState)] = 0;
    }


}
//#endif
