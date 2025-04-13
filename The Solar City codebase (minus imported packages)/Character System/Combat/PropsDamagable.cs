using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsDamagable : Damagable
{
    [SerializeField] Transform dustPosition;
    [SerializeField] float dustScale = 1;
    [SerializeField] float deathDustScale = 1;


    public override Reaction TakeDamage(Weapon weapon, RaycastHit _)
    {
        if (dead)
            return Reaction.miss;
        if (dustPosition != null)
            CombatSettings.instance.MakeDust(dustPosition.position, dustPosition.rotation, dustScale);

        /*
        Vector3 directionFromAttacker = (transform.position - weapon.owner.position).normalized;
        float pushForce = weapon.attack.onlyPushOnDeath ? 0 : weapon.attack.pushForce;
        float pushForceOnDeath = weapon.attack.pushForce - pushForce;
        */
        TakeDamageDirectly(weapon.attack.damage, weapon.owner.gameObject); //, pushForceOnDeath * directionFromAttacker);
        return Reaction.hurt;
    }


    protected override void Die(Vector3? push = null)
    {
        base.Die();
        if (dustPosition != null)
            CombatSettings.instance.MakeSphericalDust(dustPosition.position, dustPosition.rotation, deathDustScale);

        if (TryGetComponent(out MeshBreaker breaker))
            breaker.Break();
        else
            GetComponentInChildren<MeshBreaker>().Break();
        Destroy(gameObject);

    }



}
