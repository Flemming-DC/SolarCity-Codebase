using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ReColorDoors : MonoBehaviour
{
    [SerializeField] float brightnessReductionFactor = 1;
    [SerializeField] Transform NonInteractibleDoorContainer;
    [SerializeField] string[] searchNames;

    void Start()
    {
        // we delay ReColor, to spread performance requirements
        this.Delay(ReColor, 0.1f);
    }

    void ReColor()
    {
        var doors = NonInteractibleDoorContainer
            .GetComponentsInChildren<Transform>(includeInactive: true)
            .Where(tr => searchNames.Contains(tr.name));

        foreach (var door in doors)
        {
            var pivots = door
                .GetComponentsInChildren<Transform>(includeInactive: true)
                .Where(tr => tr.name.Contains("PIVOT"));

            foreach (var pivot in pivots)
            {
                pivot.GetComponentsInChildren<Renderer>(includeInactive: true)
                .ToList()
                .ForEach(r => r.material.color *= brightnessReductionFactor);
            }
        }

    }



}
