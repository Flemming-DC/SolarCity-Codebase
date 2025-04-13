using System;
using UnityEngine;
using Animancer;

public class RandomStrafe : Strafing
{
    [SerializeField] float expectedStrafeDirectionTime = 3f;
    [SerializeField] float forwardTendency = 1.8f;
    [SerializeField] float backwardwardTendency = 0.7f;

    Vector2 strafeInputDirection;

    private void Start()
    {
        strafeInputDirection = new Vector2(0, 1);
    }
    
    public override void OnEnter(object target)
    {
        getTarget = () => (Transform)target;
        getInputDirection = GetStrafeInputDirection;

        base.OnEnter(null);
    }


    Vector2 GetStrafeInputDirection() //constant probability density for direction change given no change so far, aka. exponential decay
    {
        float rand = UnityEngine.Random.Range(0f, 1f);
        if (rand < Time.deltaTime / expectedStrafeDirectionTime)
            strafeInputDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f),
                                               UnityEngine.Random.Range(-backwardwardTendency, forwardTendency)
                                               ).normalized;
        return strafeInputDirection;
    }





}
