using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3_Extensions
{

    public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(x == null ? vector.x : (float)x,
                           y == null ? vector.y : (float)y,
                           z == null ? vector.z : (float)z);
    }

    public static float VerticalAngle(this Vector3 vector)
    {
        Vector3 horizontal = new Vector3(vector.x, 0, vector.z);
        return Mathf.Atan2(vector.y, horizontal.magnitude);
    }



}
