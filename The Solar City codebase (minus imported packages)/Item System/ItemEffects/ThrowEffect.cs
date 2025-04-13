using System;
using UnityEngine;

[Serializable]
public class ThrowEffect : ItemEffect
{
    [Header("------------ Throw ------------")]
    [SerializeField] GameObject projectile;



    public override void Apply()
    {
        PlayerReferencer.CharacterComponent<IAimThrow>().TryThrow(projectile);
    }



}
