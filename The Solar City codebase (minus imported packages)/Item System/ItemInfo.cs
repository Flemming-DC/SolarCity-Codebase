using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemInfo : MonoBehaviour
{
    [SerializeField] bool usedInInventory;
    [SerializeField] Sprite transparentFrame;
    [SerializeField] Image icon;
    [SerializeField] TMP_Text itemName;
    [SerializeField] GameObject hotKey;
    [SerializeField] TMP_Text description;
    [SerializeField] AttackDisplay[] attackDisplays;

    GameObject selection;
    GameObject lastSelection;


    private void Update()
    {
        if (!usedInInventory)
            return;

        selection = EventSystem.current.currentSelectedGameObject;
        if (selection == lastSelection)
            return;

        if (selection.TryGetComponent(out InventorySlot slot) && slot.itemCount > 0)
            DisplayItemInfo(slot, slot.item);
        else
            SetActive(false);
    }



    public void DisplayItemInfo(InventorySlot slot, Item item)
    {
        SetActive(true);
        icon.sprite = item.icon;
        itemName.text = item.name;
        description.text = item.description.Replace("\n", "");
        if (slot.hasHotKey)
            InputIconSet.ShowButton(slot.hotKey, hotKey);
        else
            hotKey.SetActive(false);

        UpdateAttackDisplays(item);
    }

    public void DisplayLimitedInfo(Item item)
    {
        SetActive(true);
        icon.sprite = item.icon;
        itemName.text = item.name;
    }

    void SetActive(bool active)
    {
        foreach (Transform child in transform) 
            child.gameObject.SetActive(active);
        if (hotKey != null)
            hotKey.SetActive(active);
    }

    void UpdateAttackDisplays(Item item)
    {

        if (item.TryGetEffect(out EquipWeapon equipEffect))
        {
            AttackDisplayData[] dataSets = equipEffect.weaponPrefab.GetComponent<IWeapon>().GetAttackDisplayData();
            for (int i = 0; i < attackDisplays.Length; i++)
                attackDisplays[i].Show(dataSets[i]);
        }
        else
        {
            for (int i = 0; i < attackDisplays.Length; i++)
                attackDisplays[i].gameObject.SetActive(false);
        }
    }

}
