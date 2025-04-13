using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Fire : MonoBehaviour
{
    [SerializeField] float damagePerTime = 20;
    [SerializeField] Sound flameSound;

    void Start()
    {
        flameSound?.MakeSource(gameObject); // play on awake is on
        if (!GetComponent<Collider>().isTrigger)
            Debug.LogWarning($"The collider on {name} must be a trigger.");

    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponentInCharacter(out ADamagable damagable))
            return;
        damagable.TakeDamage(damagePerTime * Time.deltaTime, HurtType.normal);

    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponentInCharacter(out ADamagable damagable))
            return;
        damagable.TakeDamageDirectly(damagePerTime * Time.deltaTime);
    }


}
