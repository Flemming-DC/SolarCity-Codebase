using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class GameObject_Extensions
{

    public static bool IsSelected(this GameObject obj)
    {
        return EventSystem.current.currentSelectedGameObject == obj;
    }

    public static bool IsPrefab(this GameObject obj)
    {
        return obj.scene.name == null;
    }

}
