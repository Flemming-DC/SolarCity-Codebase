using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Physics_Extensions
{

    public static RaycastHit[] LinecastAll(Vector3 start, Vector3 end)
    {
        return Physics.RaycastAll(start,
                                  end - start,
                                  (end - start).magnitude);
    }


}
