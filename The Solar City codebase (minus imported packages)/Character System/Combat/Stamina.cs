using System;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    [SerializeField] float regenerationDuration = 3;
    [SerializeField] float regenerationPauseDuration = 1;

    public static event Action<float> onStaminaChanged; // <newStaminaFraction>
    static float staminaPct = 1;
    static float lastPaymentTime;
    float regenerationSpeed;

    void Awake()
    {
        if (regenerationPauseDuration > regenerationDuration)
            Debug.LogError($"regenerationPauseDuration > regenerationDuration, which is not allowed.");

        regenerationSpeed = 1f / (regenerationDuration - regenerationPauseDuration);
    }

    void Update()
    {
        if (staminaPct < 1)
            if (Time.time > lastPaymentTime + regenerationPauseDuration)
                GainStamina(regenerationSpeed * Time.deltaTime);
    }

    public static bool TryPayStaminaCost(float cost)
    {
        if (staminaPct <= 0) // (staminaPct < cost)
            return false;

        staminaPct -= cost;
        staminaPct = Math.Clamp(staminaPct, 0, 1);
        onStaminaChanged?.Invoke(staminaPct);
        lastPaymentTime = Time.time;
        return true;
    }

    static void GainStamina(float gain)
    {
        staminaPct += gain;
        staminaPct = Math.Clamp(staminaPct, 0, 1);
        onStaminaChanged?.Invoke(staminaPct);
    }


}
