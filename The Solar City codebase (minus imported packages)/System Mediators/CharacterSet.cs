using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSet : MonoBehaviour
{
    public static List<GameObject> characters { get; private set; } = new List<GameObject>();

    private void Awake()
    {
        characters.Add(transform.parent.gameObject);
    }

    private void OnDestroy()
    {
        characters.Remove(transform.parent.gameObject);
    }

}
