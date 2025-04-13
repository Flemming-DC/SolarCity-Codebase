using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTime : MonoBehaviour
{
    [SerializeField] float lifeTime = 10;

    void Start()
    {
        this.Delay(() => Destroy(gameObject), lifeTime);
    }

}
