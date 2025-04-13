using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour, ISense
{
    [SerializeField] float distance = 5;
    [SerializeField] float angle = 60;
    [SerializeField] float verticalDistance = 2.5f;
    //[SerializeField] SightType sightType;
    [SerializeField] LayerMask blockingLayers;
    [SerializeField] UpdateTimer updateTimer;

    //enum SightType { cone, wedge }
    public List<GameObject> detectedObjects { get; private set; } = new();
    Collider[] collidersInRange = new Collider[50];
    Transform model;
    int count;

    void Awake()
    {
        model = this.GetComponentInSiblings<BodyParts>().transform;
        updateTimer.Start(GetComponentInParent<CharacterReferencer>(), 0.1f, 0.4f);
    }

    void OnDestroy() => updateTimer.Stop();

    void Update()
    {
        if (updateTimer.TimeToUpdate())
            Scan();
    }

    public void Scan()
    {
        count = Physics.OverlapSphereNonAlloc(
            transform.position,
            distance,
            collidersInRange,
            LayerData.instance.characterMask,
            QueryTriggerInteraction.Collide
            );

        detectedObjects.Clear();
        for (int i = 0; i < count; i++)
        {
            GameObject obj = collidersInRange[i].gameObject;
            if (InSight(obj.transform))
                detectedObjects.Add(obj);
        }
    }

    public bool InSight(Transform target)
    {
        Vector3 displacement = target.position - transform.position;

        //if (sightType == SightType.wedge)
        if (displacement.y < -verticalDistance || displacement.y > verticalDistance)
            return false;

        return InCone(displacement) && UnBlocked(target);
    }

    bool InCone(Vector3 displacement)
    {
        if (displacement.sqrMagnitude > distance * distance)
            return false;

        float angleToObj = Vector3.Angle(displacement, model.forward);
        if (angleToObj > angle)
            return false;

        return true;
    }

    bool UnBlocked(Transform target)
    {
        Vector3 midOrigin = transform.position + verticalDistance / 2.0f * transform.up;
        Vector3 pointInTarget = target.position + verticalDistance / 2.0f * transform.up;

        return !Physics.Linecast(midOrigin, pointInTarget, blockingLayers);
    }

    public (float, float, float) GetParameters()
    {
        return (distance, angle, verticalDistance);
    }

}
