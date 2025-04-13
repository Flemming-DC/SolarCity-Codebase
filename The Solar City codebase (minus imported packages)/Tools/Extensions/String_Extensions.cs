using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class String_Extensions
{
    public static string TrimStart(this string source, string substring)
    {
        if (source.StartsWith(substring))
            return source.Remove(0, substring.Length);
        else
            return source;
    }

    public static string TrimEnd(this string source, string substring)
    {
        if (source.EndsWith(substring))
            return source.Remove(0, substring.Length);
        else
            return source;
    }

    /*
    public static string ReplaceFirst(this string source, string old, string new_)
    {
        int place = source.IndexOf(old);
        string result = source.Remove(place, old.Length).Insert(place, new_);
        return result;
    }

    public static string ReplaceLast(this string source, string old, string new_)
    {
        int place = source.LastIndexOf(old);
        string result = source.Remove(place, old.Length).Insert(place, new_);
        return result;
    }
    */
}
