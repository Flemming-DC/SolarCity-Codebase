using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ADamagable : MonoBehaviour
{
    public bool dead { get; protected set; }
    public abstract event Action OnDie;

    public abstract void TakeDamage(float damage, HurtType hurtType, Vector3? push = null);
    public abstract void TakeDamageDirectly(float damage, GameObject attacker = null, Vector3? push = null);
    public abstract void Heal(float healAmount, GameObject healer = null);

    public virtual void PlayerPretendToDie() { }
    public virtual void SetMaxHealthFactor(float multiplier) { }



}
