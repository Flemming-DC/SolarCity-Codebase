using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Print_Extensions
{
    public static void Print<T>(this T[] array, string prefix = "")
    {
        foreach (var element in array)
            Debug.Log(prefix + element);
    }


    public static void Print<T>(this List<T> list, string prefix = "")
    {
        foreach (var element in list)
            Debug.Log(prefix + element);
    }

    public static void Print<T1, T2>(this Dictionary<T1, T2> dict, string prefix = "")
    {
        foreach (var pair in dict)
            Debug.Log(prefix + $"{pair.Key}, {pair.Value}");
    }


}
