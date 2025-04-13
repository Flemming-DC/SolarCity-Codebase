using System;
using System.Collections;
using UnityEngine;
using Animancer;

[Serializable]
public class Heal : ItemEffect
{
    [Header("------------ Heal ------------")]
    [SerializeField] float healAmount;
    [SerializeField] ClipTransition healAnimation;
    [SerializeField] Sound healSound;
    [SerializeField] GameObject healingObject;
    [SerializeField] float healDelay = 2.8f;

    ADamagable playerDamagable;
    ICustomAnimation customAnimation;

    public override void Start()
    {
        playerDamagable = PlayerReferencer.CharacterComponent<ADamagable>();
        customAnimation = PlayerReferencer.CharacterComponent<ICustomAnimation>();
        healSound?.MakeSource(PlayerReferencer.player);
        
    }

    public override void Apply()
    {
        BehaviourEventCaller.BeginRoutine(HealRoutine());
    }

    IEnumerator HealRoutine()
    {
        customAnimation.TryPlay(healAnimation, healingObject, Hand.right);


        yield return new WaitWhile(() => healAnimation.State.Time < healDelay && customAnimation.isPlaying);
        if (customAnimation.isPlaying)
        {
            playerDamagable.Heal(healAmount);
            healSound?.Play(PlayerReferencer.player);
        }

    }





}
