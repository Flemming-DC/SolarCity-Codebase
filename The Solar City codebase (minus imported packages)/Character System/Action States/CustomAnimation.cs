using UnityEngine;
using Animancer;

public class CustomAnimation : ActionState, ICustomAnimation
{
    // evt.  allow rotation or some additional customization
    // option to only play on upper layer
    public bool isPlaying { get => actMachine.currentStateType == GetType(); }
    AnimancerComponent animancer;
    ActMachine actMachine;
    ScivoloMover mover;
    AWeaponEquiper equipper;

    private void Start()
    {
        animancer = this.GetComponentInSiblings<AnimancerComponent>();
        actMachine = this.GetComponentInSiblings<ActMachine>();
        mover = this.GetComponentInSiblings<ScivoloMover>();
        equipper = this.GetComponentInSiblings<WeaponHandler>().equipper;
    }


    public override void OnEnter(object input)
    {
        var (animation, toolPrefab, hand) = ((ClipTransition, GameObject, Hand))input;
        if (toolPrefab != null)
            equipper.ReplaceWeapon(hand, toolPrefab);
        mover.velocity = Vector3.zero;
        animancer.Play(animation, 0.25f)
            .Events.OnEnd = () => actMachine.SetNextState(typeof(Idle));

    }

    public override void OnExit()
    {
        equipper.BringBackWeapons();
    }



    public bool TryPlay(ClipTransition animation, GameObject toolPrefab = null, Hand hand = Hand.right)
    {
        if (actMachine.UnInterruptable())
            return false;

        actMachine.SetNextState(this, null, (animation, toolPrefab, hand));
        return true;
    }



}
