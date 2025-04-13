using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerMotion
{
    public void ApplySpeedModifier(float factor, float? duration = null);

}
