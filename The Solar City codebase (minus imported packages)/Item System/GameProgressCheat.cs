using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProgressCheat : MonoBehaviour
{
    [SerializeField] Transform cheatProgressPoint;
    [SerializeField] List<Item> possessedItems;

    void Start()
    {
        cheatProgressPoint.gameObject.SetActive(false);
        InputManager.editor.GameProgressCheat.performed += _ => Cheat();
    }

    void OnDestroy()
    {
        InputManager.editor.GameProgressCheat.performed -= _ => Cheat();
    }


    void Cheat()
    {
        if (!Application.isEditor)
            return;
        foreach (var item in possessedItems)
            Inventory.Add(item);
        PlayerReferencer.player.transform.position = cheatProgressPoint.position;
    }


}
