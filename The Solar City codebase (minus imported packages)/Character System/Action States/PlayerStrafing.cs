using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStrafing : Strafing
{
    [SerializeField] ALockOnRotator lockOnRotator;

    public override void OnEnter(object dummy)
    {
        getInputDirection = InputManager.gameplay.Walk.ReadValue<Vector2>;
        getTarget = () => lockOnRotator.target;

        base.OnEnter(null);
    }




}
