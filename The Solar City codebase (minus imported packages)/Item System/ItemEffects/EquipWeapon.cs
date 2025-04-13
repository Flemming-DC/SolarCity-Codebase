using System;
using UnityEngine;

[Serializable]
public class EquipWeapon : ItemEffect
{
    [Header("------------ EquipWeapon ------------")]
    public GameObject weaponPrefab;
    public bool onlyInLeftHand;

    public bool isTwoHanded { get; private set; }
    public Hand hand { get; set; }
    WeaponEquiper equipper;


    public override void Start()
    {
        equipper = PlayerReferencer.CharacterComponent<WeaponEquiper>();

        if (weaponPrefab == null)
        {
            Debug.LogWarning($"the weaponPrefab {weaponPrefab} is null");
            return;
        }
        else if (weaponPrefab.TryGetComponent(out IWeapon weapon))
            isTwoHanded = weapon.IsTwoHanded();
        else
            Debug.LogWarning($"the weaponPrefab {weaponPrefab} lacks a weapon component");
    }



    public override void Apply()
    {
        // evt. check if hand has been chosen
        equipper.ToggleEquipment(hand, weaponPrefab, item);

    }




}
