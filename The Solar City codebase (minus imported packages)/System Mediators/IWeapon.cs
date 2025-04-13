using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    public bool IsTwoHanded();

    public AttackDisplayData[] GetAttackDisplayData();

}


public struct AttackDisplayData
{
    public string name;
    public string description;
    public float damage;
    public float attackSpeed;
    public float windUpDuration;
    //public bool canHitMultipleTargets;
    public bool blockable;
    public bool canHitToppledFoes;
    public bool canToppleFoes;
}