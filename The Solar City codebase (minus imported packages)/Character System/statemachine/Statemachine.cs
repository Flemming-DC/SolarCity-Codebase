using System;
using System.Collections.Generic;
using UnityEngine;

public class Statemachine
{
    public event Action<State, State> BeforeStateChanged;
    public event Action<State, State> AfterStateChanged;
    public State currentState { get; private set; }
    public Type currentStateType { get => currentState?.GetType(); }
    public object currentInput { get; private set; }
    State initialState;
    Dictionary<Type, List<Transition>> transitionsByFrom = new Dictionary<Type, List<Transition>>();  
    List<Transition> transitionsFromAny = new List<Transition>();
    float lastSwitchTime;
    bool hasLoggedWarning;

    public Statemachine(State initialState_, GameObject stateContainer = null)
    {
        if (initialState_ == null)
            Debug.LogWarning($"the initialState shouldn't be null");
        initialState = initialState_;
        currentState = initialState;

        if (stateContainer != null)
            foreach(State state in stateContainer.GetComponents<State>())
                transitionsByFrom.Add(state.GetType(), new List<Transition>());

        if (!transitionsByFrom.ContainsKey(initialState_.GetType()))
            transitionsByFrom.Add(initialState_.GetType(), new List<Transition>());
    }


    public void Tick()
    {
        WarnIfStateExceedsMaxTime();
        (State nextState, object input) = GetNextStateAndInput();
        SetState(nextState, input);
        currentState?.Tick();
    }


    (State, object) GetNextStateAndInput()
    {
        if (currentState == null)
            return (initialState, null); // no input to initialState

        if (!transitionsByFrom.ContainsKey(currentState.GetType()))
        {
            Debug.LogError($"transitionsByFrom does not contain the currentState {currentState}" +
                           $".\nInstead it contains:");
            transitionsByFrom.Print();
        }

        foreach (var transistion in transitionsByFrom[currentState.GetType()])
            if (transistion.condition())
                return (transistion.to, transistion.input);

        foreach (var transistion in transitionsFromAny)
            if (transistion.condition())
                return (transistion.to, transistion.input);

        return (currentState, null); // no input to currentState
    }

    public void SetState(State nextState, object input)
    {
        if (nextState == currentState)
            return;

        BeforeStateChanged?.Invoke(currentState, nextState);
        currentState?.OnExit();
        currentState = nextState;
        currentInput = input;
        lastSwitchTime = Time.time;
        hasLoggedWarning = false;
        nextState?.OnEnter(input);
        AfterStateChanged?.Invoke(currentState, nextState);
    }

    void WarnIfStateExceedsMaxTime()
    {
        if (hasLoggedWarning)
            return;
        if (currentState.maxTime == null)
            return;
        if (Time.time <= lastSwitchTime + (float)currentState.maxTime)
            return;
        
        string characterPath = currentState.transform.parent.Path();
        Debug.LogWarning(
            $"The state {currentState} on {characterPath} has lasted for more than" +
            $" {(float)currentState.maxTime} seconds. This is probably an error.");
        hasLoggedWarning = true;
    }

    public void AddTransition(State from, State to, Func<bool> condition, object input = null)
    {
        if (to == null)
        {
            Debug.LogWarning($"Trying to add transition from {from} to null with condition {condition}. \nHowever, one cannot transition to null.");
            return;
        }

        if (from == null)
            transitionsFromAny.Add(new Transition(to, condition, input));
        else
        {
            if (!transitionsByFrom.ContainsKey(from.GetType()))
                transitionsByFrom.Add(from.GetType(), new List<Transition>());

            transitionsByFrom[from.GetType()].Add(new Transition(to, condition, input));
        }
    }

    public void SwapStateInTransitions(State oldState, State newState, object newStateInput)
    {
        if (newState == oldState)
            return;

        foreach (var transition in transitionsFromAny)
            if (transition.to == oldState)
                transition.to = newState;

        foreach (var transitionList in transitionsByFrom.Values)
            foreach (var transition in transitionList)
                if (transition.to == oldState)
                    transition.to = newState;

        if (currentState == oldState)
            SetState(newState, newStateInput);

        transitionsByFrom[newState.GetType()] = transitionsByFrom[oldState.GetType()];
        transitionsByFrom.Remove(oldState.GetType());
    }


    class Transition
    {
        public Transition(State to_, Func<bool> condition_, object input_)
        {
            to = to_;
            condition = condition_;
            input = input_;
        }

        public Func<bool> condition;
        public State to;
        public object input;
    }

}
