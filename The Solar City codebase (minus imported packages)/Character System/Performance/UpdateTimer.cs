using System;
using UnityEngine;
using ConditionalField;

[Serializable]
public class UpdateTimer
{
    [SerializeField] bool deviateFromDeafultInterval;
    [ConditionalField(nameof(deviateFromDeafultInterval))] [SerializeField] float intervalNear;
    [ConditionalField(nameof(deviateFromDeafultInterval))] [SerializeField] float intervalFar;

    float interval;
    float nextTime;
    Collider characterCollider;

    public void Start(CharacterReferencer character_, float defaultIntervalNear, float defaultIntervalFar)
    {
        if (!deviateFromDeafultInterval)
        {
            intervalNear = defaultIntervalNear;
            intervalFar = defaultIntervalFar;
        }
        interval = intervalNear;
        nextTime = Time.time + UnityEngine.Random.value * intervalNear;
        characterCollider = character_.GetComponent<CapsuleCollider>(true);

        RemotePerformanceHandler.onCharacterNearPlayerChanged += OnCharacterNearPlayerChanged;
    }



    public void Stop()
    {
        RemotePerformanceHandler.onCharacterNearPlayerChanged -= OnCharacterNearPlayerChanged;
    }

    public bool TimeToUpdate()
    {
        if (Time.time >= nextTime)
        {
            nextTime = Time.time + interval;
            return true;
        }
        else 
            return false;
    }

    

    void OnCharacterNearPlayerChanged(Collider characterCollider_, bool isNear_)
    {
        if (characterCollider_ != characterCollider)
            return;
        interval = isNear_ ? intervalNear : intervalFar;
    }

}
