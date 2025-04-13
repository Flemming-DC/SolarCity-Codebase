using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Item")]
public class Item : ScriptableObject
{
    public ItemCatagory catagory;
    public Sprite icon;
    [Multiline] public string description;
    [SerializeReference] public List<ItemEffect> itemEffects = new List<ItemEffect>();
    
    public bool allowHotKey { get => catagory == ItemCatagory.consumable || catagory == ItemCatagory.weapon; }


    private void OnEnable()
    {
        foreach (var effect in itemEffects)
            effect.item = this;
    }

    public void Use()
    {
        if (PlayerReferencer.CharacterComponent<IActMachine>().UnInterruptable())
        {
            Inventory.Add(this);
            return;
        }

        if (catagory == ItemCatagory.amulet)
        {
            AmuletEquipper.OpenChooseAmuletWindow(this);
            return;
        }

        foreach (var effect in itemEffects)
            effect.Apply();
    }

    public void StopUsing()
    {
        foreach (var effect in itemEffects)
            effect.Cease();
    }



    public bool TryGetEffect<T>(out T t) where T : ItemEffect // effects must be released for garbage collection
    {
        t = GetEffect<T>();
        return t != null;
    }


    public T GetEffect<T>() where T : ItemEffect // effects must be released for garbage collection
    {
        foreach (var effect in itemEffects)
            if (effect.GetType() == typeof(T))
                return (T)effect;

        return null;
    }

}

public enum ItemCatagory { consumable, weapon, amulet, special}

