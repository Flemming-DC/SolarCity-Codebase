using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObjectSingleton<T>
{
    static T instance_;

    public static T instance
    {
        get
        {
            if (instance_ == null)
                SetInstance();

            return instance_;
        }
    }


    static void SetInstance()
    {
        T[] assets = Resources.LoadAll<T>("");
        if (assets == null)
            throw new System.Exception($"didn't find any {typeof(T)} in any Ressources folder");
        if (assets.Length == 0)
            throw new System.Exception($"didn't find any {typeof(T)} in any Ressources folder");
        if (assets.Length > 1)
            Debug.LogWarning($"found more than one {typeof(T)} in the Ressources folders");

        instance_ = assets[0];
    }

}
