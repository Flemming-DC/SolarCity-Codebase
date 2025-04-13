using System;
using UnityEngine;

public class CharacterDamagable : Damagable
{
    [SerializeField] bool armoured;

    ActMachine actMachine;
    Blocking blocking;
    Ragdoll ragdoll;

    protected override void Start()
    {
        actMachine = this.GetComponent<ActMachine>(true);
        blocking = this.GetComponentInSiblings<Blocking>(false);
        ragdoll = this.GetComponentInSiblings<Ragdoll>();
        base.Start();
    }

    public override Reaction TakeDamage(Weapon weapon, RaycastHit hit)
    {
        if (dead)
            return Reaction.miss;
        if (actMachine.currentStateType == typeof(Dodge))
            return Reaction.miss;
        if (ragdoll.isToppled && !weapon.attack.canHitToppledFoes)
            return Reaction.miss;
        if (blocking != null)
            if (blocking.TryBlock(weapon, hit))
                return Reaction.block;
        if (armoured && weapon.attack.blockable)
        {
            weapon.Recoil(hit, true);
            return Reaction.block;
        }

        Vector3 directionFromAttacker = (transform.position - weapon.owner.position).normalized;
        float pushForce = weapon.attack.onlyPushOnDeath ? 0 : weapon.attack.pushForce;
        //float pushForceOnDeath = weapon.attack.pushForce - pushForce;
        float damage = weapon.attack.damage;
        var attackProggresion = actMachine.InState<BowShot>() ? AttackProggresion.windup
            : actMachine.GetState<AttackState>().attackProggresion;
        Hurt.StaggerData staggerData = new Hurt.StaggerData(
            weapon.attack.hurtType, 
            pushForce * directionFromAttacker,
            actMachine.currentStateType,
            attackProggresion,
            weapon.owner.gameObject);

        if (ragdoll.isToppled)
            damage *= CombatSettings.instance.toppleDamageMultiplier;
        else
            actMachine.GetState<Hurt>().TryEnter(staggerData);
        //TakeDamageDirectly(damage, weapon.owner.gameObject, pushForceOnDeath * directionFromAttacker);
        TakeDamageDirectly(damage, weapon.owner.gameObject, weapon.attack.pushForce * directionFromAttacker);
        return Reaction.hurt;
    }

    public override void TakeDamage(float damage, HurtType hurtType, Vector3? push = null)
    {
        if (dead)
            return;
        var attackProggresion = actMachine.InState<BowShot>() ? AttackProggresion.windup
            : actMachine.GetState<AttackState>().attackProggresion;
        Hurt.StaggerData staggerData = new Hurt.StaggerData(
            hurtType, 
            push ?? Vector3.zero, 
            actMachine.currentStateType,
            attackProggresion, 
            null);
        actMachine.GetState<Hurt>().TryEnter(staggerData);
        TakeDamageDirectly(damage, null, push);
    }


    protected override void Die(Vector3? push = null)
    {
        actMachine.SetState<Idle>();
        base.Die();
        ScivoloMover mover = this.GetComponentInSiblings<ScivoloMover>();
        mover.velocity += (push == null ? Vector3.zero : (Vector3)push);

        if (ragdoll == null)
            Debug.LogWarning($"couldn't find ragdoll");
        else
            ragdoll.Die();

    }

    public override void PlayerPretendToDie()
    {
        if (!transform.parent.TryGetComponent(out PlayerReferencer _))
            Debug.LogWarning($"PlayerPretendToDie is called on a non-player character");
        base.Die();
    }

    

}
