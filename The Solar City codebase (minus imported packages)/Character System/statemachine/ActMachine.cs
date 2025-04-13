using UnityEngine;
using Animancer;

public class ActMachine : StatemachineBehavior, IActMachine
{
    [SerializeField] string motionStateName;
    public State motionState { get; private set; }

    AnimancerComponent animancer;
    float nullAnimationTimer;

    protected override void Awake()
    {
        base.Awake();
        motionState = GetState(motionStateName);
        animancer = this.GetComponentInSiblings<AnimancerComponent>();

    }

    protected override void Update()
    {
        base.Update();
        FixNullAnimationBug();
    }

    public void SwitchMotionState(State newMotionState, object input = null)
    {
        SetNextState(newMotionState, motionState, input);
        motionState = newMotionState;
        
    }

    public bool UnInterruptable()
    {
        return !((ActionState)statemachine.currentState).interruptable;

    }

    void FixNullAnimationBug()
    {
        if (!Application.isEditor)
            return;
        //if (Time.timeSinceLevelLoad < 2 * Time.deltaTime)
        //    return;

        // the animancerState is harmlessly null in the start of Getup.
        if (statemachine.currentState is Getup)
            nullAnimationTimer = 0; // reset timer if no bug was detected
        if (animancer.States.Current != null)
            // the clip is harmlessly null when strafing, due to the use of the strafing mixer
            if (animancer.States.Current.Clip != null || statemachine.currentState is Strafing)
                nullAnimationTimer = 0; // reset timer if no bug was detected
        nullAnimationTimer += Time.deltaTime;
        float threshold = 0.5f;
        if (nullAnimationTimer < threshold)
            return;

        Debug.LogWarning(
            $"Encountered null animation in state {currentStateType} on {transform.parent.Path()}" +
            $"Time.time = {Time.time}" +
            $"animancer.States.Current = {animancer.States.Current}");

        SetState<Idle>();
    }



}
