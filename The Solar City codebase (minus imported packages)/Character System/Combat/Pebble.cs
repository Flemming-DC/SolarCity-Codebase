using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pebble : MonoBehaviour
{
    [SerializeField] float damage = 2;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponentInCharacter(out Damagable damagable))
            damagable.TakeDamage(damage, HurtType.normal);
        Destroy(gameObject); 
    }



}
