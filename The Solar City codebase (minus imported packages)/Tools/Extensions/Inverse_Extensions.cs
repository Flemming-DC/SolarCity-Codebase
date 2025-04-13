using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Inverse_Extensions
{

    public static Dictionary<T, int> GetInverse<T>(this T[] array)
    {
        Dictionary<T, int> inverse = new Dictionary<T, int>();
        for (int i = 0; i < array.Length; i++)
            inverse.Add(array[i], i);

        return inverse;
    }

    public static Dictionary<T, int> GetInverse<T>(this List<T> list)
    {
        Dictionary<T, int> inverse = new Dictionary<T, int>();
        for (int i = 0; i < list.Count; i++)
            inverse.Add(list[i], i);

        return inverse;
    }


    public static Dictionary<T2, T1> GetInverse<T1, T2>(this IDictionary<T1, T2> dict)
    {
        Dictionary<T2, T1> inverse = new Dictionary<T2, T1>();
        foreach (var pair in dict)
            inverse.Add(pair.Value, pair.Key);

        return inverse;
    }

}
