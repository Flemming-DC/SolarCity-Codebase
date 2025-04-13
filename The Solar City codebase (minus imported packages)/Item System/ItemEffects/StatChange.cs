using UnityEngine;

public class StatChange : ItemEffect
{
    [Header("------------ StatChange ------------")]

    [SerializeField] StatID stat;
    [SerializeField] float changefactor = 1;
    [SerializeField] float duration = 1;

    enum StatID {moveSpeed, attackSpeed, damage, bloodScale}



    public override void Apply()
    {
        switch (stat)
        {
            case StatID.moveSpeed:
                PlayerReferencer.CharacterComponent<IPlayerMotion>()
                    .ApplySpeedModifier(changefactor, duration);
                break;
            case StatID.attackSpeed:
                PlayerReferencer.CharacterComponent<IAttackState>()
                    .attackSpeedModifier.ApplyFactor(changefactor, duration);
                break;
            case StatID.damage:
                PlayerReferencer.CharacterComponent<IAttackState>()
                    .attackDamageModifier.ApplyFactor(changefactor, duration);
                break;
            case StatID.bloodScale:
                PlayerReferencer.CharacterComponent<IAttackState>()
                    .bloodScaleModifier.ApplyFactor(changefactor, duration);
                break;
            default:
                Debug.LogWarning($"Cannot recognize the stat {stat}");
                break;

        }
    }


}
