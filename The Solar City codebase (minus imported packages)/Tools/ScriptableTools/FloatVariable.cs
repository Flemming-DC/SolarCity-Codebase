// ----------------------------------------------------------------------------
// Unite 2017 - Game Architecture with Scriptable Objects
// 
// Author: Ryan Hipple
// Date:   10/04/17
// ----------------------------------------------------------------------------

using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/FloatVariable")]
public class FloatVariable : ScriptableObject
{
    #if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
    #endif
    public float value;
    /*
    public event Action<float, float> OnValueChanged;
    public float? minValue;
    public float? maxValue;
    
    float floatMinValue;
    float floatMaxValue;

    private void OnEnable()
    {
        if (minValue == null)
            minValue = float.MinValue;
        floatMinValue = (float)minValue;

        if (maxValue == null)
            maxValue = float.MaxValue;
        floatMaxValue = (float)maxValue;
    }
    */
    public void SetValue(float value_)
    {
        //float oldValue = value;
        value = value_;
        //value = Mathf.Clamp(value_, floatMinValue, floatMaxValue);
        //OnValueChanged?.Invoke(oldValue, value);
    }

    public void SetValue(FloatVariable value_)
    {
        SetValue(value_.value);
    }

    public void ApplyChange(float amount)
    {
        SetValue(value + amount);
    }

    public void ApplyChange(FloatVariable amount)
    {
        SetValue(value + amount.value);
    }


}