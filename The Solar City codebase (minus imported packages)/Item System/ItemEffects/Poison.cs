using System;
using System.Collections;
using UnityEngine;
using Animancer;

[Serializable]
public class Poison : ItemEffect
{
    // hurtAnim and ui indicator
    [Header("------------ Poison ------------")]
    [SerializeField] float totalDamageAmount = 100;
    [SerializeField] float poisonDuration = 60;
    [SerializeField] ClipTransition drinkAnimation;
    [SerializeField] Sound drinkSound;
    [SerializeField] GameObject bottle;
    [SerializeField] float poisonDelay = 2.8f;

    ADamagable playerDamagable;
    ICustomAnimation customAnimation;

    public override void Start()
    {
        playerDamagable = PlayerReferencer.CharacterComponent<ADamagable>();
        customAnimation = PlayerReferencer.CharacterComponent<ICustomAnimation>();
        drinkSound?.MakeSource(PlayerReferencer.player);
    }

    public override void Apply()
    {
        BehaviourEventCaller.BeginRoutine(HealRoutine());
    }

    IEnumerator HealRoutine()
    {
        customAnimation.TryPlay(drinkAnimation, bottle, Hand.right);


        yield return new WaitWhile(() => drinkAnimation.State.Time < poisonDelay && customAnimation.isPlaying);
        if (!customAnimation.isPlaying)
            yield break;

        drinkSound?.Play(PlayerReferencer.player);
        yield return new WaitForSeconds(0.5f);
        Messenger.ShowPoison();

        float healPerTime = totalDamageAmount / poisonDuration;
        float healAmountSoFar = 0;

        playerDamagable.TakeDamage(healPerTime, HurtType.normal);
        while (healAmountSoFar < totalDamageAmount)
        {
            playerDamagable?.TakeDamageDirectly(healPerTime);
            healAmountSoFar += healPerTime;
            yield return new WaitForSeconds(1);
        }

    }


}
