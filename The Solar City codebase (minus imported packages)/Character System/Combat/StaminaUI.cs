using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField] Image fill;

    void Start()
    {
        SetStaminaUI(1);
        Stamina.onStaminaChanged += SetStaminaUI;
    }

    void OnDestroy()
    {
        Stamina.onStaminaChanged -= SetStaminaUI;
    }


    void SetStaminaUI(float staminaPct)
    {
        if (staminaPct < 0)
            Debug.LogWarning($"StaminaUI received a negative stamina percent.");

        fill.fillAmount = staminaPct;
    }


}
