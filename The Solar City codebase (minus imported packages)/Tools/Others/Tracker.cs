using System;
using System.Reflection;
using UnityEngine;

public static class Tracker
{
    // type variable { get => variable_; set => Tracker.OnChanged(ref variable_, value); }

    public static void OnChanged<T>(ref T old, T new_)
    {
        Debug.Log($"oldValue = {old}, newValue = {new_}");
        Debug.Log(new System.Diagnostics.StackTrace());
        old = new_;
    }


    public static void PrintTraceback()
    {
        Debug.Log(new System.Diagnostics.StackTrace());
    }


    /*
    event Action<float, float> OnChanged;
    object parent;
    string nameOfTracked;
    
    public Tracker(object parent_, string nameOfTracked_)
    {
        parent = parent_;
        nameOfTracked = nameOfTracked_;
    }

    private void OnEnable() => OnChanged += PrintStack;
    private void OnDisable() => OnChanged -= PrintStack;


    void PrintStack(float old, float new_)
    {
        UnityEngine.Debug.Log(new StackTrace());
    }
    */

}
