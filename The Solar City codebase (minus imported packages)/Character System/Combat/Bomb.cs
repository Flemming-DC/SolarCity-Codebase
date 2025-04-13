using UnityEngine;

public class Bomb : MonoBehaviour
{
    // evt. drop force and damage with distance (put keep a minimum effective distance)
    [SerializeField] float damage = 200;
    [SerializeField] float force = 10;
    [SerializeField] float radius = 6;
    [SerializeField] float lifeTime = 10;
    [SerializeField] float MaxTickPitch = 1.1f;
    [SerializeField] float MaxTickVolume = 0.4f;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] Sound tick;
    [SerializeField] Sound explosionSound;

    AudioSource audioSource;

    private void Start()
    {
        audioSource = tick?.MakeSource(gameObject);
        tick?.Play(gameObject);
        this.Delay(() => audioSource.pitch = MaxTickPitch, 0.75f * lifeTime);
        this.Delay(() => audioSource.volume = MaxTickVolume, 0.75f * lifeTime);
        this.Delay(Explode, lifeTime);
    }




    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }


    void Explode()
    {
        GameObject explosionInstance = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosionSound.MakeSource(explosionInstance);
        explosionSound.Play(explosionInstance);

        PushAndDamage();
        Destroy(gameObject);
    }

    void PushAndDamage()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, radius); // evt. mask by dammagable

        foreach (var target in targets)
        {
            Vector3 push = force * (target.transform.position - transform.position).normalized;
            if (target.TryGetComponentInCharacter(out Damagable damagable))
                damagable.TakeDamage(damage, HurtType.stronglyPushed, push);
            else if (target.TryGetComponent(out Rigidbody rb))
                rb.AddForce(push, ForceMode.Impulse);

        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }


}
