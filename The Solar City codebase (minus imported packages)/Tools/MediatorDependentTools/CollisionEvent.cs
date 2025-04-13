using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEvent : MonoBehaviour
{
    [SerializeField] bool onlyDetectPlayer = true;
    [SerializeField] bool onlyOnce = true;
    public UnityEvent<Collider> onTriggerEnter;
    public UnityEvent<Collider> onTriggerExit;


    private void Start()
    {
        var collider = GetComponent<Collider>();
        if (collider == null || !collider.isTrigger)
            Debug.LogError($"The collider on {transform.Path()} is either null or not a trigger");
    }

    void OnTriggerEnter(Collider other)
    {
        if (onlyDetectPlayer)
            if (other.gameObject != PlayerReferencer.player)
                return;
        if (!enabled)
            return;

        onTriggerEnter?.Invoke(other);
        if (onlyOnce)
            enabled = false;
    }


    void OnTriggerExit(Collider other)
    {
        if (onlyDetectPlayer)
            if (other.gameObject != PlayerReferencer.player)
                return;
        if (!enabled)
            return;

        onTriggerExit?.Invoke(other);
        if (onlyOnce)
            enabled = false;
    }


}
