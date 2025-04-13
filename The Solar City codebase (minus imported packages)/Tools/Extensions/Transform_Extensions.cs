using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Transform_Extensions
{

    public static List<Transform> GetAncestors(this Transform transform)
    {
        List<Transform> ancestors = new List<Transform>();
        Transform ancestor = transform;

        while (ancestor.parent != null)
        {
            ancestor = ancestor.parent;
            ancestors.Add(ancestor);
        }
        return ancestors;

    }

    public static string Path(this Transform transform)
    {
        Transform ancestor = transform;
        string path = transform.name;

        while (ancestor.parent != null)
        {
            ancestor = ancestor.parent;
            path = ancestor.name + '\\' + path;
        }
        return path;
    }

}
