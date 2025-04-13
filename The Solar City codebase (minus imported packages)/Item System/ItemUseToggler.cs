using UnityEngine;
using UnityEngine.InputSystem;

public class ItemUseToggler : MonoBehaviour
{

    void Start()
    {
        InputManager.always.ToggleItemUsage.performed += _ => SetActionMap(InputManager.itemUsage);
        InputManager.always.ToggleItemUsage.canceled += _ => SetActionMap(InputManager.gameplay);
    }


    

    void SetActionMap(InputActionMap map)
    {
        if (InputManager.UI.enabled)
            return;

        InputManager.SetActionMap(map);
    }




}
