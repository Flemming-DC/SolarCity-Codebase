using System;
using UnityEngine;
using Animancer;

public class CombatAnimationFunctions: MonoBehaviour
{
    [SerializeField] bool debug;
    [SerializeField] float debugSlowdown = 0.1f;

    public event Action<Attack> onStartSwing;
    public event Action<Attack, bool> onExitSwing; //the bool marks whether the animation is playing forwards, when exitting the swing part of the attack animation
    AnimancerComponent animancer;
    WeaponHandler weaponHandler;
    ActMachine actMachine;
    AttackState attackState;

    private void Start()
    {
        animancer = this.GetComponentInSiblings<AnimancerComponent>();
        weaponHandler = this.GetComponentInSiblings<WeaponHandler>();
        actMachine = this.GetComponentInSiblings<ActMachine>();
        attackState = this.GetComponentInSiblings<AttackState>();
    }
    /*
    private void Update()
    {
        if (Keyboard.current.bKey.wasPressedThisFrame && InputManager.editor.enabled)
            debug = !debug;
    }
    */

    //called from inspector by name on all attack animations
    public void EnableDamageCollider() //OnStartSwing
    {
        if (actMachine?.currentStateType != typeof(AttackState))
            return;
        if (debug)
            Time.timeScale = debugSlowdown;

        Weapon weapon = weaponHandler.weapons[attackState.hand];

        if (animancer.States.Current.Speed >= 0)
        {
            onStartSwing?.Invoke(weapon.attack);
            CombatSettings.instance.swingSound?.Play(weapon.gameObject); // evt. introduce a small delay before playing sound
            animancer.States.Current.Speed = attackState.attackSpeedMultiplier * weapon.attack.swingSpeed;
            attackState.attackProggresion = AttackProggresion.swing;

            StartCoroutine(weapon.DetectHits());
        }
        else
        {
            attackState.attackProggresion = AttackProggresion.windup;
            onExitSwing?.Invoke(weapon.attack, false);
        }
    }

    //called from inspector by name on all attack animations
    public void DisableDamageCollider() //OnEndSwing
    {
        Time.timeScale = 1f;

        if (actMachine?.currentStateType != typeof(AttackState))
            return;
        if (animancer.States.Current.Speed < 0)
            return;

        attackState.attackProggresion = AttackProggresion.rewind;
        Weapon weapon = weaponHandler.weapons[attackState.hand];
        onExitSwing?.Invoke(weapon.attack, true);
        animancer.States.Current.Speed = attackState.attackSpeedMultiplier * weapon.attack.rewindSpeed;
        Invoke(nameof(MarkSwingAsFinished), weapon.attack.delayDuringComboAfterThisAttack);
    }

    //called from inspector by name on some attack animations
    public void OnRecoilFinished()
    {
        if (actMachine?.currentStateType != typeof(AttackState))
            return;
        if (animancer.States.Current.Speed >= 0)
            return;

        actMachine.SetNextState(typeof(Idle));
    }


    void MarkSwingAsFinished()
    {
        if (attackState.attackProggresion == AttackProggresion.rewind)
            attackState.swingIsFinishedWithDelay = true;
    }


    #region unused animation functions
    public void FootL() { }
    public void FootR() { }
    #endregion

}
