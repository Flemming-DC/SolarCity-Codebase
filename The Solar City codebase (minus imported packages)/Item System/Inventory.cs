using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public static Dictionary<Item, InventorySlot> slots = new Dictionary<Item, InventorySlot>();
    

    public static void Add(Item item)
    {
        slots[item].AddItem();
    }

    public static void Remove(Item item)
    {
        slots[item].RemoveItem();
    }

    private void OnDestroy()
    {
        slots.Clear();
    }

}
