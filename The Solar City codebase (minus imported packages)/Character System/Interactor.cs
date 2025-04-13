using UnityEngine;
using System.Linq;

public class Interactor : MonoBehaviour
{
    // if you wish to add interact animation, then turn this into a state. 
    // if so, then consider whether the animation may depend upon the interactable.
    // evt. reduce range when interacting from behind
    // don't jump when interacting
    [SerializeField] float range = 2;
    [SerializeField] bool drawGizmo = true;

    GameObject hint;
    Collider[] collidersNearby = new Collider[50];

    private void Start()
    {
        hint = InputIconSet.ShowButton(InputManager.gameplay.Interact);
        InputManager.gameplay.Interact.performed += _ => Interact();
    }

    private void Update()
    {
        int colliderCount = Physics.OverlapSphereNonAlloc(
            transform.position, range, collidersNearby);

        bool interactableNearby = false;
        for (int i=0; i<colliderCount; i++)
        {
            if (!collidersNearby[i].TryGetComponent(out Interactable interactible))
                continue;
            if (!interactible.enabled)
                continue;

            interactableNearby = true;
            break;
        }
        /*
        bool interactableNearby = Physics
            .OverlapSphere(transform.position, range)
            .Any(c => c.TryGetComponent(out Interactable i) && i.enabled);
        */
        hint.SetActive(interactableNearby && InputManager.gameplay.enabled);
    }


    void Interact()
    {
        Transform[] interactables = Physics
            .OverlapSphere(transform.position, range)
            .Where(c => c.TryGetComponent(out Interactable i) && i.enabled)
            .Select(c => c.transform)
            .ToArray();

        if (interactables.Length > 0)
            GetNearest(interactables).GetComponent<Interactable>().Interact();
    }


    Transform GetNearest(Transform[] transforms)
    {
        float minSqrDistance = float.MaxValue;
        float sqrDistance;
        Transform closestTransfrom = null;

        foreach (var transform_ in transforms)
        {
            sqrDistance = (transform.position - transform_.position).sqrMagnitude;
            if (sqrDistance < minSqrDistance)
            {
                minSqrDistance = sqrDistance;
                closestTransfrom = transform_;
            }
        }
        return closestTransfrom;
    }


    private void OnDrawGizmosSelected()
    {
        if (!drawGizmo)
            return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }

}
