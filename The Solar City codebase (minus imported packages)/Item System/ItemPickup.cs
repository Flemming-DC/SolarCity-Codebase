using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour, Interactable
{
    public Item item;
    [SerializeField] float gizmoRadius = 0.3f;

    public static event Action<Item> onPickup;
    

    public void Interact()
    {
        onPickup?.Invoke(item);
        Inventory.Add(item);
        Destroy(gameObject);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, gizmoRadius);
    }

}
