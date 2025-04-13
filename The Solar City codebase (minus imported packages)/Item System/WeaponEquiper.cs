using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeaponEquiper : AWeaponEquiper
{
    [SerializeField] Transform leftHandTransform;
    [SerializeField] Transform rightHandTransform;
    [SerializeField] GameObject bareFistPrefab;

    public override event Action<Hand, GameObject> AfterWeaponObjectChanged; //<Hand, newEquipmentObject>
    public static event Action<Item, Dictionary<Hand, Item>> OnWeaponToggled; //<weapon, previousWeapons>
    Dictionary<Hand, GameObject> weaponObjects = new Dictionary<Hand, GameObject>();
    Dictionary<Hand, Transform> handTransforms = new Dictionary<Hand, Transform>();
    Dictionary<Hand, Item> weaponItems = new Dictionary<Hand, Item>();
    Dictionary<Hand, GameObject> temporaryTools = new Dictionary<Hand, GameObject>();

    private void Awake()
    {
        foreach (var hand in (Hand[])Enum.GetValues(typeof(Hand)))
            weaponObjects.Add(hand, null);

        handTransforms.Add(Hand.left, leftHandTransform);
        handTransforms.Add(Hand.right, rightHandTransform);
        UnEquip(Hand.left);
        UnEquip(Hand.right);
        if (bareFistPrefab.GetComponent<IWeapon>().IsTwoHanded())
            Debug.LogError("the bareFist equipment must not be twohanded");

    }

    public void ToggleEquipment(Hand hand, GameObject weaponPrefab, Item weaponItem)
    {
        if (weaponObjects.Values.Any(w => !w.activeSelf))
            return;

        OnWeaponToggled?.Invoke(weaponItem, weaponItems);

        if (weaponItems[Hand.left] == weaponItem)
            UnEquip(Hand.left);
        else if (weaponItems[Hand.right] == weaponItem)
            UnEquip(Hand.right);
        else
            Equip(hand, weaponPrefab, weaponItem);

    }


    public void Equip(Hand hand, GameObject weaponPrefab, Item weaponItem)
    {
        bool requireItem = (weaponPrefab != bareFistPrefab);
        if (requireItem && weaponItem == null)
        {
            Debug.LogWarning($"requireItem = {requireItem}, but weaponItem = {weaponItem}");
            return;
        }

        weaponItems[hand] = weaponItem;
        if (IsTwoHandedWeapon(weaponPrefab))
            hand = Hand.right;

        RemovePreviousEquipment(hand, weaponPrefab);
        weaponObjects[hand] = Instantiate(weaponPrefab, handTransforms[hand]);
        AfterWeaponObjectChanged?.Invoke(hand, weaponObjects[hand]);
    }

    public override void UnEquip(Hand hand)
    {
        Equip(hand, bareFistPrefab, null);
    }

    void RemovePreviousEquipment(Hand hand, GameObject newWeaponPrefab)
    {
        if (weaponObjects[hand] != null)
            Destroy(weaponObjects[hand]);

        Hand otherHand = (hand == Hand.right) ? Hand.left : Hand.right;
        if (weaponObjects[otherHand] != null)
            if (IsTwoHandedWeapon(weaponObjects[otherHand]) || IsTwoHandedWeapon(newWeaponPrefab))
                UnEquip(otherHand);
    }


    bool IsTwoHandedWeapon(GameObject weaponPrefab)
    {
        if (weaponPrefab.TryGetComponent(out IWeapon weapon))
            return weapon.IsTwoHanded();
        else
            return false;
    }


    public override GameObject ReplaceWeapon(Hand hand, GameObject toolPrefab)
    {
        if (IsTwoHandedWeapon(weaponObjects[Hand.left]) || IsTwoHandedWeapon(weaponObjects[Hand.right]))
        {
            weaponObjects[Hand.left].SetActive(false);
            weaponObjects[Hand.right].SetActive(false);
        }
        else
        {
            weaponObjects[hand].SetActive(false);
        }

        temporaryTools[hand] = Instantiate(toolPrefab, handTransforms[hand]);
        return temporaryTools[hand];
    }

    public override void BringBackWeapons()
    {
        if (temporaryTools.ContainsKey(Hand.left))
            Destroy(temporaryTools[Hand.left]);
        if (temporaryTools.ContainsKey(Hand.right))
            Destroy(temporaryTools[Hand.right]);

        this.Delay(() => weaponObjects[Hand.left].SetActive(true), 0.2f);
        this.Delay(() => weaponObjects[Hand.right].SetActive(true), 0.2f);
    }


}
