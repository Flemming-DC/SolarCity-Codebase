using System;
using System.Collections.Generic;
using UnityEngine;

public class DirectedStrafing : Strafing
{

    public override void OnEnter(object input)
    {
        (Transform target, Func<Vector2> inputDirection) = ((Transform, Func<Vector2>))input;
        getTarget = () => target;
        getInputDirection = inputDirection;

        base.OnEnter(null);
    }


}
