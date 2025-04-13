using System;
using UnityEngine;
using Animancer;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObject/Attack")]
public class Attack : ScriptableObject
{
    [SerializeField] string displayName;
    [TextArea] [SerializeField] string description;
    public ClipTransition animation;

    [Header("Combat Stats")]
    public Stat damage = 20;
    public Stat windUpSpeed = 1;
    public Stat swingSpeed = 1;
    public Stat rewindSpeed = 1;
    [Tooltip("OBS: if this parameter is larger than the time it takes to finish the attack, then the combo ends")]
    public float delayDuringComboAfterThisAttack = 0;
    [Tooltip("stamina is only used by player")] public float staminaCost = 0.2f;

    public bool canRotateDuringWindUp;
    public bool canHitMultipleTargets;
    public bool blockable = true;
    public bool isBlocking;
    public bool canHitToppledFoes;
    public bool forceOneHandedness;
    public bool unRecoilable;
    public bool onlyPushOnDeath = true;
    public float pushForce = 2;
    public HurtType hurtType;
    public Hand hand = Hand.right; // only relevant for AI, since player determines hand through button

    [Header("Sweep Stats")]
    public float sweepHeight = 1.5f;
    public float sweepSizeAngle = 90;
    [Tooltip("  0 = right to left\n  90 = top to down\n  180 = left to right\n  270 = -90 = down to top")]
    public float sweepOrientationAngle;
    [Tooltip("sweepAngleOffset = 0 means that the sweep cone is symmetric around the start and final direction.")]
    public float sweepAngleOffset;
    public int hitDetectionRayCount = 20;

    float hittingTime;

    public AttackDisplayData GetAttackDisplayData(bool isInCombo)
    {
        AttackDisplayData data = new AttackDisplayData();
        data.name = displayName;
        data.description = description;
        data.damage = damage;
        data.attackSpeed = GetAttackSpeed(isInCombo);
        data.windUpDuration = GetWindUpDuration();
        //data.canHitMultipleTargets = canHitMultipleTargets;
        data.blockable = blockable;
        data.canHitToppledFoes = canHitToppledFoes;
        data.canToppleFoes = (hurtType == HurtType.pushed || hurtType == HurtType.stronglyPushed);

        return data;
    }

    public float GetWindUpDuration()
    {
        float windUpTime = -1;
        string eventName = nameof(CombatAnimationFunctions.EnableDamageCollider);
        foreach (var event_ in animation.Clip.events)
        {
            if (event_.functionName == eventName)
                windUpTime = event_.time;
        }
        if (windUpTime == -1)
        {
            if (!isBlocking && name != "Falling Attack")
                Debug.LogError(
                    $"could not calculate {nameof(GetWindUpDuration)} on {name}. " +
                    $"Have you created the {eventName} event on this attack?");
            return 0;
        }
        return windUpTime / windUpSpeed;

    }

    public float GetAttackSpeed(bool isInCombo)
    {
        float windUpTime = -1;
        float swingTime = -1;
        string eventName_1 = nameof(CombatAnimationFunctions.EnableDamageCollider);
        string eventName_2 = nameof(CombatAnimationFunctions.DisableDamageCollider);
        foreach (var event_ in animation.Clip.events)
        {
            if (event_.functionName == eventName_1)
                windUpTime = event_.time;
            else if (event_.functionName == eventName_2)
                swingTime = event_.time - windUpTime;
        }
        if (windUpTime == -1 || swingTime == -1)
        {
            if (!isBlocking)
                Debug.LogError(
                    $"could not calculate {nameof(GetAttackSpeed)} on {name}. " +
                    $"Have you created the {eventName_1} and {eventName_2} events on this attack?");
            return 0;
        }

        float rewindTime = animation.Clip.length - swingTime - windUpTime;
        if (isInCombo)
            rewindTime = Mathf.Min(rewindTime, delayDuringComboAfterThisAttack);
        float attackDuration = windUpTime / windUpSpeed + swingTime / swingSpeed + rewindTime / rewindSpeed;
        //Debug.Log($"{attackDuration} = {windUpTime} / {windUpSpeed} + {swingTime} / {swingSpeed} + {rewindTime} / {rewindSpeed}");
        return 1 / attackDuration;
    }

    public bool IsPassedHittingTime()
    {
        if (hittingTime == 0)
            SetupHittingTime();
        return animation.State.Time > hittingTime;

    }

    public void SetupHittingTime()
    {
        float windUpTime = -1;
        float swingTime = -1;
        string eventName_1 = nameof(CombatAnimationFunctions.EnableDamageCollider);
        string eventName_2 = nameof(CombatAnimationFunctions.DisableDamageCollider);
        foreach (var event_ in animation.Clip.events)
        {
            if (event_.functionName == eventName_1)
                windUpTime = event_.time;
            else if (event_.functionName == eventName_2)
                swingTime = event_.time - windUpTime;
        }
        if (windUpTime == -1 || swingTime == -1)
        {
            if (!isBlocking)
                Debug.LogError(
                    $"could not calculate {nameof(SetupHittingTime)} on {name}. " +
                    $"Have you created the {eventName_1} and {eventName_2} events on this attack?");
        }

        hittingTime = windUpTime + 0.5f * swingTime;
    }

}
