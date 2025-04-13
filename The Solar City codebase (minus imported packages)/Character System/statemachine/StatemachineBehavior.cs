using System;
using System.Collections.Generic;
using UnityEngine;

public class StatemachineBehavior : MonoBehaviour
{
    [SerializeField] protected GameObject stateContainer;
    [SerializeField] string initialStateName;

    public Dictionary<Type, State> states { get; private set; } = new Dictionary<Type, State>();
    public Statemachine statemachine { get; private set; }
    public Type currentStateType { get => statemachine.currentState?.GetType(); }
    public State initialState { get; set; }
    bool isDestroyed;

    protected virtual void Awake()
    {
        foreach (var state in stateContainer.GetComponents<State>())
            states.Add(state.GetType(), state);

        initialState = GetState(initialStateName);
        statemachine = new Statemachine(initialState, stateContainer);
    }

    private void OnDestroy()
    {
        isDestroyed = true; // this will hopefully remove strafing error
    }

    protected virtual void Update()
    {
        if (gameObject == null)
            isDestroyed = true;
        if (isDestroyed)
            return;
        statemachine.Tick();
    }


    public State GetState(string name)
    {
        State state = (State)stateContainer.GetComponent(name);
        if (state == null)
            Debug.LogWarning($"Failed to find state with name {name} on {stateContainer}.");
        return state;
    }
    public T GetState<T>() where T : State
    {
        return (T)states[typeof(T)];
    }
    public bool InState<T>() where T : State
    {
        return currentStateType == typeof(T);
    }


    public void SetState<T>(object input = null) where T : State
    {
        if (!states.ContainsKey(typeof(T)))
            Debug.LogWarning($"Failed to find state {typeof(T).Name} on {transform.parent.Path()}.");

        statemachine.SetState(states[typeof(T)], input);
    }
    public void SetNextState(Type nextState, Type from = null, object input = null)
    {
        if (!states.ContainsKey(nextState))
            Debug.LogWarning($"Failed to find state {nextState.Name} on {transform.parent.Path()}.");
        if (from != null)
            if (!states.ContainsKey(from))
                Debug.LogWarning($"SetNextState recevied a from-state, which aren't among the known states.");

        if (from == null || from == currentStateType)
            statemachine.SetState(states[nextState], input);
    }

    public void SetNextState(State nextState, State from = null, object input = null)
    {
        if (!states.ContainsValue(nextState))
            Debug.LogWarning($"Failed to find state {nextState.name} on {transform.parent.Path()}.");
        if(from != null)
            if (!states.ContainsKey(from.GetType()))
                Debug.LogWarning($"SetNextState recevied a from-state, which aren't among the known states.");

        if (from == null || from == statemachine.currentState)
            statemachine.SetState(nextState, input);
    }

    public void AddTransitionFromAny(State to, Func<bool> condition, object input = null)
    {
        if (!states.ContainsValue(to))
            Debug.LogWarning($"the destination state {to} wasn't found on {transform.parent.Path()}.");

        statemachine.AddTransition(null, to, condition, input);
    }


    public void AddTransition(State from, Type to, Func<bool> condition, object input = null)
    {
        if (!states.ContainsValue(from))
            Debug.LogWarning($"the origin state {from} wasn't found on {transform.parent.Path()}.");
        if (!states.ContainsKey(to))
            Debug.LogWarning($"the destination state {to} wasn't found on {transform.parent.Path()}.");

        statemachine.AddTransition(from, states[to], condition, input);
    }


}
