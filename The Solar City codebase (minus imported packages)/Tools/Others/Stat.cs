using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    public float baseValue; // never set this from any other class than Stat

    float factor = 1;
    float additiveBoost = 0;
    public float value { get => factor * baseValue + additiveBoost; }
    bool hasFactor;
    bool hasBoost;

    public void ApplyFactor(float newFactor, float? duration = null)
    {
        hasFactor = true;
        factor *= newFactor;
        if (duration == null)
            return;
        Action reset = () => { if (hasFactor) factor /= newFactor; };
        BehaviourEventCaller.Delay(reset, duration);
    }

    public void ApplyBoost(float newBoost, float? duration = null)
    {
        hasBoost = true;
        additiveBoost += newBoost;
        if (duration == null)
            return;
        Action reset = () => { if (hasBoost) additiveBoost -= newBoost; };
        BehaviourEventCaller.Delay(reset, duration);
    }

    public void SetFactor(float newFactor)
    {
        hasFactor = false;
        factor = newFactor;
    }
    public void SetBoost(float newBoost)
    {
        hasBoost = true;
        additiveBoost = newBoost;
    }


    public static implicit operator float(Stat stat)
    {
        return stat.value;
    }

    public static implicit operator Stat(float baseValue)
    {
        Stat stat = new Stat();
        stat.baseValue = baseValue;
        return stat;
    }

    public override string ToString()
    {
        return value.ToString();
    }

}
