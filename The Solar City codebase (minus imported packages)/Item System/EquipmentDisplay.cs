using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDisplay : MonoBehaviour
{
    [SerializeField] Image leftWeaponIcon;
    [SerializeField] Image rightWeaponIcon;
    [SerializeField] Image leftAmuletIcon;
    [SerializeField] Image rightAmuletIcon;

    Sprite leftHandIcon;
    Sprite rightHandIcon;
    Sprite amuletIcon;
    bool lastWeaponWasTwohanded;


    private void Start()
    {
        leftHandIcon = leftWeaponIcon.sprite;
        rightHandIcon = rightWeaponIcon.sprite;
        amuletIcon = leftAmuletIcon.sprite;
        if (leftAmuletIcon.sprite != rightAmuletIcon.sprite)
            Debug.LogWarning($"leftAmuletIcon.sprite != rightAmuletIcon.sprite, but the equipmentDisplay needs some slight modofications to deal with that");

        WeaponEquiper.OnWeaponToggled += UpdateWeaponDisplay;
        AmuletEquipper.OnAmuletsChanged += UpdateAmuletDisplay;
    }

    private void OnDestroy()
    {
        WeaponEquiper.OnWeaponToggled -= UpdateWeaponDisplay;
        AmuletEquipper.OnAmuletsChanged -= UpdateAmuletDisplay;
    }



    void UpdateWeaponDisplay(Item weapon, Dictionary<Hand, Item> weaponItems)
    {
        EquipWeapon equipWeapon = weapon.GetEffect<EquipWeapon>();

        if (weaponItems.ContainsValue(weapon))
        {
            if (equipWeapon.isTwoHanded)
            {
                SetIcon(Hand.left, leftHandIcon);
                SetIcon(Hand.right, rightHandIcon);
            }
            else if (weaponItems[Hand.left] == weapon)
                SetIcon(Hand.left, leftHandIcon);
            else if (weaponItems[Hand.right] == weapon)
                SetIcon(Hand.right, rightHandIcon);

            return;
        }


        if (lastWeaponWasTwohanded)
        {
            Hand otherHand = (equipWeapon.hand == Hand.right) ? Hand.left : Hand.right;
            Sprite otherHandIcon = (otherHand == Hand.right) ? rightHandIcon : leftHandIcon;
            SetIcon(otherHand, otherHandIcon);
        }
        lastWeaponWasTwohanded = equipWeapon.isTwoHanded;

        if (equipWeapon.isTwoHanded)
        {
            leftWeaponIcon.sprite = equipWeapon.item.icon;
            rightWeaponIcon.sprite = equipWeapon.item.icon;
        }
        else
            SetIcon(equipWeapon.hand, equipWeapon.item.icon);

    }

    void SetIcon(Hand hand, Sprite icon)
    {
        if (hand == Hand.right)
            rightWeaponIcon.sprite = icon;
        else
            leftWeaponIcon.sprite = icon;
    }


    void UpdateAmuletDisplay(List<Item> amulets)
    {
        leftAmuletIcon.sprite = amulets[0] == null ? amuletIcon : amulets[0].icon;
        rightAmuletIcon.sprite = amulets[1] == null ? amuletIcon : amulets[1].icon;
    }

}
