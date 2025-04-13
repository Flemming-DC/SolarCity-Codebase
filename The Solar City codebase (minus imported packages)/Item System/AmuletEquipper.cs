using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmuletEquipper : MonoBehaviour
{
    [SerializeField] GameObject amuletIconContainer;
    [SerializeField] float closeDuration = 0.5f;

    public static event Action<List<Item>> OnAmuletsChanged;
    static AWindow amuletWindow;
    static Item newAmulet;
    static List<Image> amuletIcons;
    static List<Item> amulets;
    static AmuletEquipper instance;

    private void Awake()
    {
        instance = this;
        amuletWindow = GetComponent<AWindow>();
        amuletIcons = amuletIconContainer.GetComponentsInDirectChildren<Image>();
        amulets = new List<Item>(new Item[amuletIcons.Count]);
    }

    
    public static void OpenChooseAmuletWindow(Item newAmulet_)
    {
        if (newAmulet_.catagory != ItemCatagory.amulet)
        {
            Debug.LogWarning($"{newAmulet_.name} isn't an amulet");
            return;
        }
        if (amulets.Contains(newAmulet_))
        {
            UnEquip(newAmulet_);
            return;
        }

        newAmulet = newAmulet_;
        if (amulets[0] == null)
            instance.OnAmuletChosen(0);
        else if (amulets[1] == null)
            instance.OnAmuletChosen(1);
        else
            amuletWindow.Open(true);
    }

    public void OnAmuletChosen(int index) // called from a UnityEvent on a button
    {
        amulets[index]?.StopUsing();
        amulets[index] = newAmulet;
        foreach (var effect in newAmulet.itemEffects) // this cannot be replaced with item.Use(), when ChooseAmuletToReplace is called from item.Use()
            effect.Apply();

        OnAmuletsChanged?.Invoke(amulets);
        amuletIcons[index].sprite = newAmulet.icon;
        if (amuletWindow.gameObject.activeSelf)
            this.Delay(() => amuletWindow.Close(true), closeDuration);
    }


    static void UnEquip(Item amulet)
    {
        amulet.StopUsing();

        int index = amulets[0] == amulet ? 0 : 1; // we assume that amulet is in amulets
        amulets[index] = null;
        amuletIcons[index].sprite = null;

        OnAmuletsChanged?.Invoke(amulets);
    }



}
