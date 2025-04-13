using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GetComponent_Extensions
{

    #region GetComponent from component
    public static T GetComponent<T>(this Component component, bool warning) where T : Component
    {
        if (warning)
        {
            if (component.TryGetComponent(out T t))
                return t;
            else
            {
                Debug.LogWarning($"didn't find {typeof(T)} on {component.gameObject}");
                return null;
            }
        }
        else
            return component.GetComponent<T>();
    }

    public static T GetComponentInDirectChildren<T>(this Component component, bool warning = true) //where T : Component
    {
        for (int i = 0; i < component.transform.childCount; i++)
        {
            if (component.transform.GetChild(i).TryGetComponent(out T t))
                return t;
        }

        if (warning)
            Debug.LogWarning($"didn't find {typeof(T)} in the direct children of {component.gameObject}");
        return default(T);
    }

    public static T GetComponentInCharacter<T>(this Component component, bool warning = true) //where T : Component
    {
        if (component.TryGetComponent(out T t))
            return t;
        else
            return component.GetComponentInDirectChildren<T>(warning);
    }

    public static T GetComponentInSiblings<T>(this Component component, bool warning = true) where T : Component
    {
        if (component.transform.parent == null)
        {
            Debug.LogWarning($"{component.name} has no parent and therefore no siblings.");
            return null;
        }

        return component.transform.parent.GetComponentInDirectChildren<T>(warning);
    }

    public static List<T> GetComponentsInDirectChildren<T>(this Component component, bool warning = true) where T : Component
    {
        List<T> results = new List<T>();
        for (int i = 0; i < component.transform.childCount; i++)
            results.AddRange(component.transform.GetChild(i).GetComponents<T>());

        if (warning && results.Count == 0)
            Debug.LogWarning($"didn't find {typeof(T)} in the direct children of {component.gameObject}");
        return results;
    }

    public static List<T> GetComponentsInSiblings<T>(this Component component, bool warning = true) where T : Component
    {
        if (component.transform.parent == null)
        {
            Debug.LogWarning($"{component.name} has no parent and therefore no siblings.");
            return null;
        }

        return component.transform.parent.GetComponentsInDirectChildren<T>(warning);
    }

    public static bool TryGetComponentInCharacter<T>(this Component component, out T t) where T : Component
    {
        if (component.TryGetComponent(out t))
            return true;
        else
        {
            t = component.GetComponentInDirectChildren<T>(false);
            return t != null;
        }
    }

    #endregion


    #region GetComponent from gameObject
    public static T GetComponent<T>(this GameObject gameObject, bool warning) where T : Component
        => gameObject.transform.GetComponent<T>(warning);

    public static T GetComponentInDirectChildren<T>(this GameObject gameObject, bool warning = true) where T : Component
        => gameObject.transform.GetComponentInDirectChildren<T>(warning);

    public static T GetComponentInCharacter<T>(this GameObject gameObject, bool warning = true) //where T : Component
        => gameObject.transform.GetComponentInCharacter<T>(warning);

    public static T GetComponentInSibling<T>(this GameObject gameObject, bool warning = true) where T : Component
        => gameObject.transform.GetComponentInSiblings<T>(warning);

    public static List<T> GetComponentsInDirectChildren<T>(this GameObject gameObject, bool warning = true) where T : Component
        => gameObject.transform.GetComponentsInDirectChildren<T>(warning);

    public static List<T> GetComponentsInSiblings<T>(this GameObject gameObject, bool warning = true) where T : Component
        => gameObject.transform.GetComponentsInSiblings<T>(warning);

    public static bool TryGetComponentInCharacter<T>(this GameObject gameObject, out T t) where T : Component
        => gameObject.transform.TryGetComponentInCharacter<T>(out t);


    #endregion


}

