using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoInteractible : MonoBehaviour, Interactable
{
    [SerializeField] GameObject InfoPanel;


    public void Interact()
    {
        ClosableMessenger.Show(InfoPanel);
        Destroy(gameObject);
    }


}
