using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    //[SerializeField] AWindow mainMenu;
    [SerializeField] Button defaultButton;

    private void Start()
    {
        //mainMenu.Open(false);
        InputManager.SetActionMap(InputManager.UI);
        defaultButton.Select();
        //this.Delay(() => defaultButton.Select());

    }

}
