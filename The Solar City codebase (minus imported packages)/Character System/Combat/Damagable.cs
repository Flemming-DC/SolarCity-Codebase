using System;
using UnityEngine;
using ConditionalField;

public class Damagable : ADamagable
{
    [SerializeField] bool makeHealthBar = true;
    [ConditionalField(nameof(makeHealthBar))] [SerializeField] bool setHealthBarHeightManually;
    [ConditionalField(nameof(makeHealthBar))] [SerializeField] GameObject healthBarPrefab;
    [ConditionalField(nameof(setHealthBarHeightManually))] [SerializeField] float setHealthBarHeight = 0.8f;
    [SerializeField] Stat maxHealth = 100;

    public float MaxHealth { get => maxHealth;}
    public float health { get; protected set; }
    public override event Action OnDie;
    public event Action<GameObject, float, float> OnHealthChanged; //attacker, oldHealth, newHealth
    GameObject healthBarObject;

    protected virtual void Start()
    {
        health = maxHealth;
        if (makeHealthBar)
            Invoke(nameof(MakeHealthBar), Time.deltaTime);
    }

    public virtual Reaction TakeDamage(Weapon weapon, RaycastHit _)
    {
        // evt. make pushforce
        TakeDamageDirectly(weapon.attack.damage, weapon.owner.gameObject);
        return Reaction.hurt;
    }

    public virtual Reaction TakeDamage(
        float damage,
        GameObject attacker = null,
        HurtType hurtType = HurtType.noReaction,
        float push = 0,
        bool onlyPushOnDeath = false)
    {
        // evt. check for dodge
        // evt. blood effect

        Vector3 directionFromAttacker = (transform.position - attacker.transform.position).normalized;
        float pushStrength = onlyPushOnDeath ? 0 : push;
        float pushForceOnDeath = push - pushStrength;

        GetComponent<Rigidbody>().AddForce(pushStrength * directionFromAttacker, ForceMode.Impulse);
        TakeDamageDirectly(damage, attacker, pushForceOnDeath * directionFromAttacker);
        return Reaction.hurt;
    }

    public override void TakeDamage(float damage, HurtType hurtType, Vector3? push = null)
    {
        if (dead)
            return;
        GetComponent<Rigidbody>().AddForce(push ?? Vector3.zero, ForceMode.Impulse);
        TakeDamageDirectly(damage, null, push);
    }

    public override void TakeDamageDirectly(float damage, GameObject attacker = null, Vector3? push = null)
    {
        if (dead)
            return;
        float oldHealth = health;
        health = Mathf.Max(oldHealth - damage, 0);

        OnHealthChanged?.Invoke(attacker, oldHealth, health);
        if (health <= 0)
            Die(push);
    }

    public override void Heal(float healAmount, GameObject healer = null)
    {
        if (dead)
            return;
        float oldHealth = health;
        health = Mathf.Min(oldHealth + healAmount, maxHealth);
        OnHealthChanged?.Invoke(healer, oldHealth, health);
    }


    protected virtual void Die(Vector3? push = null)
    {
        dead = true;
        OnDie?.Invoke();
    }



    void MakeHealthBar()
    {
        Transform healthBarCarrier = (transform.parent == null) ? transform : transform.parent;
        healthBarObject = Instantiate(healthBarPrefab, healthBarCarrier);
        HealthBar healthBar = healthBarObject.GetComponent<HealthBar>();
        CapsuleCollider collider = GetComponentInParent<CapsuleCollider>();
        float height = setHealthBarHeightManually ? setHealthBarHeight : collider.height;
        healthBar.SetupHealthBar(this, height);
    }

    public override void SetMaxHealthFactor(float multiplier)
    {
        float healthPct = health / maxHealth;
        maxHealth.SetFactor(multiplier);
        health = healthPct * maxHealth;
    }

    public enum Reaction { hurt, miss, block}
}
