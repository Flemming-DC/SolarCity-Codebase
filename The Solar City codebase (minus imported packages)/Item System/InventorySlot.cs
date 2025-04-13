using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Item item;
    [SerializeField] Image icon;
    [SerializeField] TMP_Text counter;
    [SerializeField] GameObject hotkeyObject;
    [SerializeField] TMP_Text handText;

    public int itemCount { get; private set; }
    public bool discovered { get; private set; }
    public InputAction hotKey { get; set; }
    public bool hasHotKey { get => hasHotKeyDict[InputManager.device]; }
    Dictionary<string, bool> hasHotKeyDict = new Dictionary<string, bool>();

    void Start()
    {
        if (Inventory.slots.ContainsKey(item))
            Debug.LogWarning($"There seems to be multiple slots with the same item");
        if (Inventory.slots.Select(pair => pair.Key.name).Contains(item.name))
            Debug.LogWarning($"There seems to be multiple items named {item.name}");
        Inventory.slots.Add(item, this);

        hotKey = InputManager.itemUsage.UseItem.Clone();
        foreach (InputControlScheme controlScheme in InputManager.inputAsset.controlSchemes)
            hasHotKeyDict[controlScheme.name] = false;
        hotKey.Enable();

        icon.sprite = item.icon;
        if (item.TryGetEffect(out EquipWeapon equipEffect))
            SetHand(Hand.right, equipEffect, false);
        if (SaveData.loaded)
            Load();

        //hotKey.performed += _ => UseItem(() => InputManager.itemUsage.enabled && hasHotKey);
        hotKey.performed += UseItemByHotKey;
        InputManager.UI.UseItem.performed += UseItemFromInventory;
        //InputManager.UI.UseItem.performed += _ => UseItem(() => gameObject.IsSelected() && !Rebinder.isRebinding);
        InputManager.UI.RemoveHotKey.performed += _ => { if (gameObject.IsSelected()) RemoveHotkey(); };
        if (item.allowHotKey)
            InputManager.UI.AssignHotKey.performed += _ => AssignHotKey();
        Rebinder.onRebindComplete += OnRebindComplete;

        UpdateVisuals();
    }


    private void OnDestroy()
    {
        Rebinder.onRebindComplete -= OnRebindComplete;
        hotKey.performed -= UseItemByHotKey;
    }

    public void AddItem()
    {
        itemCount++;
        discovered = true;
        Save();
        UpdateVisuals();
    }

    public void RemoveItem()
    {
        itemCount--;
        Save();
        UpdateVisuals();
    }


    void UseItemByHotKey(InputAction.CallbackContext dummy)
    {
        if (!InputManager.itemUsage.enabled)
            return;
        if (!hasHotKey)
            return;
        UseItem();
    }

    void UseItemFromInventory(InputAction.CallbackContext dummy)
    {
        if (!gameObject.IsSelected())
            return;
        if (Rebinder.isRebinding)
            return;
        UseItem();
    }

    void UseItem()
    {
        if (itemCount == 0)
        {
            Messenger.ShowHint($"No more {item.name}'s");
            return;
        }

        if (item.catagory == ItemCatagory.consumable)
            RemoveItem();
        item.Use();
    }


    void AssignHotKey()
    {
        if (gameObject != EventSystem.current.currentSelectedGameObject)
            return;
        Rebinder.Rebind(hotKey);
    }

    void OnRebindComplete(InputAction action)
    {
        if (action != hotKey)
            return;

        if (isActiveAndEnabled)
            StartCoroutine(BrieflyDisableUIInput());
        hasHotKeyDict[InputManager.device] = true;
        UpdateVisuals();
        InputIconSet.ShowButton(hotKey, hotkeyObject);

        RemoveConflictingBindings();

        if (item.TryGetEffect(out EquipWeapon equipEffect))
        {
            if (equipEffect.onlyInLeftHand)
                SetHand(Hand.left, equipEffect, false);
            else if (equipEffect.isTwoHanded)
                SetHand(Hand.right, equipEffect, false);
            else
                HandSelector.CallWithHand((Hand hand) => SetHand(hand, equipEffect, true));
            handText.transform.parent.gameObject.SetActive(true);
        }
        Save();
    }

    void RemoveConflictingBindings()
    {
        for (int i = 0; i < hotKey.bindings.Count; i++)
        {
            foreach (var slot in Inventory.slots.Values)
            {
                if (slot == this)
                    continue;
                if (slot.hotKey.bindings[i].ToDisplayString() == "") // i.e. slot has no hotKey for this device
                    continue;
                if (slot.hotKey.bindings[i].ToDisplayString() != hotKey.bindings[i].ToDisplayString())
                    continue;
                slot.RemoveHotkey();

                Messenger.ShowHint($"{slot.item.name} has lost its hotkey, due to a hotkey conflict");
            }
        }
    }

    void SetHand(Hand hand, EquipWeapon equipEffect, bool save)
    {
        equipEffect.hand = hand;

        if (equipEffect.isTwoHanded)
            handText.text = "B";
        else if (hand == Hand.right)
            handText.text = "R";
        else
            handText.text = "L";

        if (save)
            Save();
    }

    void UpdateVisuals()
    {
        counter.text = itemCount.ToString();
        counter.transform.parent.gameObject.SetActive(item.catagory == ItemCatagory.consumable);
        hotkeyObject.SetActive(hasHotKey && itemCount > 0);
        icon.color = itemCount > 0 ? Color.white : Color.black;
    }

    public void RemoveHotkey()
    {
        if (hotKey.bindings.Count == 0)
            return;
        for (int i = hotKey.bindings.Count - 1; i >= 0; i--)
            hotKey.ApplyBindingOverride("");
        hasHotKeyDict[InputManager.device] = false;
        hotkeyObject.SetActive(false);
        Save();
    }

    IEnumerator BrieflyDisableUIInput()
    {
        InputManager.UI.UseItem.Disable();
        InputManager.UI.AssignHotKey.Disable();
        InputManager.UI.RemoveHotKey.Disable();
        InputManager.UI.GoToRightTab.Disable();
        InputManager.UI.GoToLeftTab.Disable();

        yield return new WaitForSeconds(0.3f);

        InputManager.UI.UseItem.Enable();
        InputManager.UI.AssignHotKey.Enable();
        InputManager.UI.RemoveHotKey.Enable();
        InputManager.UI.GoToRightTab.Enable();
        InputManager.UI.GoToLeftTab.Enable();
    }


    void Save()
    {
        if (item.catagory != ItemCatagory.consumable)
            SaveData.file.Add(item.name + ".itemCount", itemCount);
        SaveData.file.Add(item.name + ".discovered", discovered);
        SaveData.file.Add(item.name + ".bindingsJson", hotKey.SaveBindingOverridesAsJson());
        SaveData.file.Add(item.name + ".handText.text", handText.text);
        if (item.TryGetEffect(out EquipWeapon equipEffect))
            SaveData.file.Add(item.name + ".equipEffect.hand", (int)equipEffect.hand);
        foreach (var key in hasHotKeyDict.Keys)
            SaveData.file.Add(item.name + ".hasHotKeyDict." + key, hasHotKeyDict[key]);
    }

    void Load()
    {
        if (item.catagory != ItemCatagory.consumable)
            itemCount = SaveData.file.GetInt(item.name + ".itemCount");
        discovered = SaveData.file.GetBool(item.name + ".discovered");
        hotKey.LoadBindingOverridesFromJson(SaveData.file.GetString(item.name + ".bindingsJson"));
        handText.text = SaveData.file.GetString(item.name + ".handText.text");
        if (item.TryGetEffect(out EquipWeapon equipEffect))
            equipEffect.hand = (Hand)SaveData.file.GetInt(item.name + ".equipEffect.hand");

        List<string> keys = new List<string>(hasHotKeyDict.Keys);
        foreach (var key in keys)
            hasHotKeyDict[key] = SaveData.file.GetBool(item.name + ".hasHotKeyDict." + key);
        if (hasHotKeyDict.Values.Contains(true))
            InputIconSet.ShowButton(hotKey, hotkeyObject);
    }



}
