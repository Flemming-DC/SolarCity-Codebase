using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Dictionary_Extensions
{
    public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
    {
        if (key == null)
            return defaultValue;

        TValue value;
        return dict.TryGetValue(key, out value) ? value : defaultValue;
    }

    public static bool IsNullOrEmpty(this IDictionary dictionary)
    {
        return (dictionary == null || dictionary.Count < 1);
    }




}
