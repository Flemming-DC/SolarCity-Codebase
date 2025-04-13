using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AWeaponEquiper : MonoBehaviour
{

    public abstract event Action<Hand, GameObject> AfterWeaponObjectChanged; //<hand, Item, newEquipmentObject>

    public abstract void UnEquip(Hand hand);

    public abstract GameObject ReplaceWeapon(Hand hand, GameObject toolPrefab);
    
    public abstract void BringBackWeapons();

}
