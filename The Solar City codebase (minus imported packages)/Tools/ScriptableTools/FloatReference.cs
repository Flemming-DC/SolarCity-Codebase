// ----------------------------------------------------------------------------
// Unite 2017 - Game Architecture with Scriptable Objects
// 
// Author: Ryan Hipple
// Date:   10/04/17
// ----------------------------------------------------------------------------

using System;
using UnityEngine;

[Serializable]
public class FloatReference
{
    public bool useConstant = true;
    public float constantValue;
    public FloatVariable variable;
    public event Action<float> OnValueChanged;

    public FloatReference()
    { }

    public FloatReference(float value)
    {
        useConstant = true;
        constantValue = value;
    }

    public float value
    {
        get { return useConstant ? constantValue : variable.value; }
        set 
        {
            float oldValue = useConstant ? constantValue : variable.value;
            OnValueChanged?.Invoke(oldValue);

            if (useConstant) 
                constantValue = value; 
            else 
                variable.value = value;
        }
    }

    public static implicit operator float(FloatReference reference)
    {
        return reference.value;
    }



}