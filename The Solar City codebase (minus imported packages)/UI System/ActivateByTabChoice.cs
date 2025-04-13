using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// used to show/hide UI elements depending on which page is shown in the tabGroup
public class ActivateByTabChoice : MonoBehaviour
{
    [SerializeField] List<TabButton> activationButtons;


    void Start()
    {
        TabGroup.OnTabChanged += OnTabChanged;
    }

    private void OnDestroy()
    {
        TabGroup.OnTabChanged -= OnTabChanged;

    }

    private void OnTabChanged(TabGroup _, TabButton button)
    {
        gameObject.SetActive(activationButtons.Contains(button));
    }
}
