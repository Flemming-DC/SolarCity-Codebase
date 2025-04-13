using UnityEngine;
using Animancer;

public static class Animancer_Extensions
{

    public static bool IsFinished(this AnimancerComponent animancer, float? finishTime = null)
    {
        var animState = animancer.States.Current;
        if (animState == null)
        {
            Debug.LogWarning($"animancer.States.Current on {animancer.transform.Path()} is null");
            return false;
        }

        if (finishTime == null)
            finishTime = animState.NormalizedEndTime;

        return animState.Speed >= 0
               ? animState.NormalizedTime >= finishTime
               : animState.NormalizedTime <= 0; // possibly use startTime

    }

}
