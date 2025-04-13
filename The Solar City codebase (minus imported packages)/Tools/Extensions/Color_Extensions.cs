using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Color_Extensions 
{

    public static Color With(this Color color, float? r = null, float? g = null, float? b = null, float? a = null)
    {
        return new Color(
            r == null ? color.r : (float)r,
            g == null ? color.g : (float)g,
            b == null ? color.b : (float)b,
            a == null ? color.a : (float)a);
    }


}
