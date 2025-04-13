using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public Dictionary<Hand, Weapon> weapons { get; private set; } = new Dictionary<Hand, Weapon>();
    public AWeaponEquiper equipper { get; private set; }
    AnimationUpdater animationUpdater;
    BodyParts bodyParts;

    void Awake()
    {
        equipper = this.GetComponentInSiblings<AWeaponEquiper>(false);
        bodyParts = this.GetComponentInSiblings<BodyParts>();
        InitializeWeapon(Hand.left);
        InitializeWeapon(Hand.right);
        animationUpdater = this.GetComponentInSiblings<AnimationUpdater>();
        if(equipper != null)
            equipper.AfterWeaponObjectChanged += OnWeaponChanged;
    }

    private void OnDestroy()
    {
        if (equipper != null)
            equipper.AfterWeaponObjectChanged -= OnWeaponChanged;
    }

    public void ReplaceNullWithBareFist()
    {
        if (equipper != null)
        {
            if (weapons[Hand.left] == null)
                equipper.UnEquip(Hand.left);
            if (weapons[Hand.right] == null)
                equipper.UnEquip(Hand.right);
        }
        else
        {
            if (weapons[Hand.left] == null)
                Instantiate(CombatSettings.instance.bareFist, bodyParts.leftHand);
            if (weapons[Hand.right] == null)
                Instantiate(CombatSettings.instance.bareFist, bodyParts.rightHand);
        }
    }

    void OnWeaponChanged(Hand hand, GameObject newEquipment)
    {
        if (hand != Hand.right && hand != Hand.left)
            return;

        weapons[Hand.left].DestroyGizmos();
        weapons[Hand.right].DestroyGizmos();
        Weapon weapon = newEquipment.GetComponent<Weapon>();

        if (weapon.IsTwoHanded())
        {
            weapons[Hand.right] = weapon;
            weapons[Hand.left] = weapon;
        }
        else
        {
            weapons[hand] = weapon;
        }
        animationUpdater.SetAnimations(weapons[Hand.right].animations);
        UpdateIK();

    }


    void InitializeWeapon(Hand hand)
    {
        Weapon weapon = bodyParts.GetHand(hand).GetComponentInDirectChildren<Weapon>(false);
        if (weapon == null)
        {
            GameObject weaponObject = Instantiate(CombatSettings.instance.bareFist, bodyParts.GetHand(hand));
            weapon = weaponObject.GetComponent<Weapon>(true);
        }
        weapons.Add(hand, weapon);
    }

    public void UpdateIK()
    {
        var twoHandingIK = this.GetComponentInSiblings<TwoHandingIK>();

        if (weapons[Hand.right].IsTwoHanded())
            twoHandingIK.ActivateIK(true, weapons[Hand.right].leftHandGrip);
        else if (weapons[Hand.left].IsTwoHanded())
            twoHandingIK.ActivateIK(true, weapons[Hand.left].leftHandGrip);
        else
            twoHandingIK.ActivateIK(false);


    }


}

