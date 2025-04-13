using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class ItemPickupUI : MonoBehaviour
{
    [SerializeField] AWindow firstTimeWindow;
    [SerializeField] AWindow afterwardsWindow;
    [SerializeField] TMP_Text assignHotkeyText;
    [SerializeField] Button firstTimeAssignHotKey;
    [SerializeField] Button firstTimeMoveOn;

    Item item;
    ItemInfo firstTimeInfo;
    ItemInfo afterwardsInfo;

    private void Start()
    {
        firstTimeInfo = firstTimeWindow.GetComponent<ItemInfo>();
        afterwardsInfo = afterwardsWindow.GetComponent<ItemInfo>();
        //InputManager.UI.AssignHotKey.performed += _ => AssignHotKey();
        ItemPickup.onPickup += Display;
        Rebinder.onRebindComplete += OnRebindComplete;

    }

    private void OnDestroy()
    {
        ItemPickup.onPickup -= Display;
        Rebinder.onRebindComplete -= OnRebindComplete;
    }


    public void Display(Item item_)
    {
        if (!Inventory.slots[item_].discovered && item_.allowHotKey)
        {
            firstTimeWindow.SetDefaultButton(firstTimeAssignHotKey);
            firstTimeWindow.Open(true);
            firstTimeInfo.DisplayItemInfo(Inventory.slots[item_], item_);
        }
        else
        {
            afterwardsWindow.Open(true);
            afterwardsInfo.DisplayLimitedInfo(item_);
        }
        item = item_;
    }


    public void AssignHotKey()
    {
        if (!firstTimeInfo.gameObject.activeSelf)
            return;

        Rebinder.Rebind(Inventory.slots[item].hotKey);
        assignHotkeyText.text = "Rebinding ...";
    }

    void OnRebindComplete(InputAction action)
    {
        if (item == null)
            return;
        if (action != Inventory.slots[item].hotKey)
            return;

        this.Delay(firstTimeInfo.DisplayItemInfo, Inventory.slots[item], item);
        assignHotkeyText.text = "Assign Hotkey";

        if (firstTimeWindow.gameObject.activeInHierarchy)
        {
            if (HandSelector.isChoosingHand)
                firstTimeWindow.SetDefaultButton(firstTimeMoveOn);
            else
                firstTimeMoveOn.Select();
        }
    }


}
