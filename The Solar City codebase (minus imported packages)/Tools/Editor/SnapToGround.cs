using UnityEditor;
using UnityEngine;

public class SnapToGround
{
    #if UNITY_EDITOR
    [MenuItem("Custom/Snap Center To Ground %h")]
    public static void CenterToGround()
    {
        foreach (var transform in Selection.transforms)
        {
            TransformToGround(transform);
            transform.position += transform.localScale.y / 2 * Vector3.up;
        }
    }


    [MenuItem("Custom/Snap Bottom To Ground %g")]
    public static void BottomToGround()
    {
        foreach (var transform in Selection.transforms)
            TransformToGround(transform);

    }
    #endif


    public static void TransformToGround(Transform transform)
    {
        float minHeight = -1000;
        RaycastHit nearestHit = default(RaycastHit);
        RaycastHit[] hits = Physics.RaycastAll( transform.position + Vector3.up, 
                                                Vector3.down, 
                                                10f, 
                                                LayerData.instance.terrainMask, 
                                                QueryTriggerInteraction.Ignore);
        foreach (var hit in hits)
        {
            if (hit.collider.transform.IsChildOf(transform))
                continue;
            if (hit.point.y < minHeight)
                continue;

            minHeight = hit.point.y;
            nearestHit = hit;
        }
        if (hits.Length > 0)
            transform.position = nearestHit.point;
    }





}
