using UnityEngine;

public class Arrow : Weapon
{
    [Header("---------------- Arrow Settings ----------------")]
    [SerializeField] Transform model;
    [SerializeField] Transform arrowTip;
    [SerializeField] Attack arrowAttack;
    [SerializeField] float speed = 10;
    [SerializeField] float lifeTime = 10;

    CharacterDamagable shooter;
    Rigidbody rb;
    bool hasHit;


    public void Fire(CharacterDamagable shooter_, Vector3 direction)
    {
        // setup
        shooter = shooter_;
        rb = GetComponent<Rigidbody>();
        attack = arrowAttack;

        // seperating arrow from character
        transform.parent = null;
        Vector3 modelPosition = model.position;
        transform.position = modelPosition;
        model.position = modelPosition;
        model.localRotation = Quaternion.Euler(90, 0, 0); // this makes the arrow point forward, not upward
        
        // initiating movement
        transform.rotation = Quaternion.LookRotation(direction);
        rb.velocity = speed * direction;
        GetComponent<TrailRenderer>().enabled = true;

        this.Delay(() => Destroy(gameObject), lifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.IsInLayerMask(LayerData.instance.hitableMask))
            return;
        if (shooter == null)
            return;
        if (other.TryGetComponentInCharacter(out CharacterDamagable damagable))
            if (damagable == shooter)
                return;
        if (hasHit)
            return;

        hasHit = true;

        if (other.IsInLayerMask(LayerData.instance.terrainMask))
        {
            Recoil(new RaycastHit());
        }
        else if (other.TryGetComponentInCharacter(out damagable))
        {
            RaycastHit dummy = new RaycastHit();
            Damagable.Reaction reaction = damagable.TakeDamage(this, dummy);

            // evt. make the arrow stick by setting the parent to the nearest bone and deplaying rb.velocity = zero
            //damagable.TakeDamage(damage, HurtType.normal);
            if (reaction == Damagable.Reaction.hurt)
                Destroy(gameObject);

        }

    }

    public override void Recoil(RaycastHit _, bool __ = false)
    {
        CombatSettings.instance.MakeSparks(arrowTip.position);
        CombatSettings.instance.hitUndamagableSound?.Play(gameObject);

        GetComponent<TrailRenderer>().enabled = false;
        model.GetComponent<Collider>().isTrigger = false;
        rb.useGravity = true;
    }

}
