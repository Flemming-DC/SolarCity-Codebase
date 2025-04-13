/*
using System.Collections.Generic;
using UnityEngine;

public class Identifiable : MonoBehaviour
{
    public Id id;

    static Dictionary<Id, GameObject> objects = new Dictionary<Id, GameObject>();


    public static GameObject Get(Id id)
    {
        if (objects.IsNullOrEmpty())
        {
            foreach(var identifiable in FindObjectsOfType<Identifiable>(includeInactive: true))
            {
                // evt. check that each id occurs exactly once and that it has the right name
                //string idName = Enum.GetName(typeof(Id), Id.LockedDoorCanvas);
                //print(identifiable.name + ", " + idName);
                objects.Add(identifiable.id, identifiable.gameObject);
            }
        }

        return objects[id];
    }

    public static T Get<T>(Id id) where T : Component
    {
        return Get(id).GetComponent<T>();
    }



}
public enum Id {  }
*/