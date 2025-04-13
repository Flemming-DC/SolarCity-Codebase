using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] float lifeTime = 10;

    void Start()
    {
        Invoke(nameof(Die), lifeTime);
    }

    void Die()
    {
        Destroy(gameObject);
    }

}
