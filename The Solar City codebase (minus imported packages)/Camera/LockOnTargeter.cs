using System.Collections.Generic;
using UnityEngine;
using System;

public class LockOnTargeter : MonoBehaviour
{
    [SerializeField] float lockOnRange = 30;
    [SerializeField] float angleWeight = 3;
    [SerializeField] float distanceWeight = 1;
    [SerializeField] float visionBlockingRadius = 1f;
    [SerializeField] GameObject player;

    public float lockOnLookAtHeight { private get; set; }
    List<Transform> nearbyTargetables = new List<Transform>();
    Dictionary<Transform, float> angles = new Dictionary<Transform, float>();
    Dictionary<Transform, float> distances = new Dictionary<Transform, float>();
    Transform target;
    Vector2 screenCenter;
    Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        screenCenter = new Vector2(0.5f * mainCamera.pixelWidth, 0.5f * mainCamera.pixelHeight);
    }

    public Transform FindInitialTarget(Vector3 cameraForward)
    {
        UpdateNearbyTargetables();
        UpdateAngles(cameraForward);
        UpdateDistances();

        float minMisMatch = 360;
        float misMatch;
        target = null;
        foreach (var targetable in nearbyTargetables)
        {
            misMatch = angleWeight * angles[targetable] + distanceWeight * distances[targetable];

            if (misMatch < minMisMatch)
            {
                minMisMatch = misMatch;
                target = targetable;
            }
        }
        return target;
    }


    public Transform SwitchTarget(Transform followTransform,  Vector2 switchDirection)
    {
        UpdateNearbyTargetables();
        nearbyTargetables.Remove(target);

        float minMismatch = float.PositiveInfinity;
        float misMatch;
        bool isInSwitchDirection;
        Vector2 targetableScreenDirection;

        foreach (var targetable in nearbyTargetables)
        {
            targetableScreenDirection = mainCamera.WorldToScreenPoint(targetable.position + lockOnLookAtHeight * Vector3.up);
            targetableScreenDirection -= screenCenter;
            isInSwitchDirection = Vector3.Dot(targetableScreenDirection, switchDirection) > 0;
            if (!isInSwitchDirection)
                continue;
            
            misMatch = targetableScreenDirection.sqrMagnitude;
            bool isBehind = Vector3.Dot(targetable.position + lockOnLookAtHeight * Vector3.up - followTransform.position, followTransform.forward) < 0;
            if (isBehind)
                misMatch += 10E9f;
            // evt. use distance in the mismatch calculation;
            if (misMatch < minMismatch)
            {
                minMismatch = misMatch;
                target = targetable;
            }
        }
        return target;
    }


    void UpdateNearbyTargetables()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, lockOnRange, LayerData.instance.characterMask);
        
        nearbyTargetables.Clear();
        foreach (var collider in colliders)
        {
            if (collider.gameObject == player)
                continue;
            if (VisionIsBlocked(collider.transform.position))
                continue;

            nearbyTargetables.Add(collider.transform);
        }

    }


    void UpdateAngles(Vector3 cameraForward)
    {
        angles.Clear();
        float angle;
        foreach (var targetable in nearbyTargetables)
        {
            angle = Vector3.Angle(cameraForward, //target.position - transform.position,
                                  targetable.position - transform.position);
            angles.Add(targetable, angle);
        }
    }

    void UpdateDistances()
    {
        distances.Clear();
        float distance;
        foreach (var targetable in nearbyTargetables)
        {
            distance = Vector3.Distance(targetable.position, transform.position);
            //sqrDistance = Vector3.SqrMagnitude(targetable.position - transform.position);
            distances.Add(targetable, distance);
        }
    }


    bool VisionIsBlocked(Vector3 sightDestination)
    {
        Func<Vector3, bool> partiallyBlocked = (offset) => Physics.Linecast(
            transform.position + offset + visionBlockingRadius * Vector3.up, 
            sightDestination + offset + visionBlockingRadius * Vector3.up, 
            LayerData.instance.terrainMask);

        if (!partiallyBlocked(Vector3.zero))
            return false;
        if (!partiallyBlocked(visionBlockingRadius * Vector3.up))
            return false;
        if (!partiallyBlocked(visionBlockingRadius * Vector3.down))
            return false;
        if (!partiallyBlocked(visionBlockingRadius * Vector3.left))
            return false;
        if (!partiallyBlocked(visionBlockingRadius * Vector3.right))
            return false;
        
        return true;
    }

}
