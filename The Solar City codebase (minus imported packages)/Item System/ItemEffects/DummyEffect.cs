using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEffect : ItemEffect
{
    [Header("------------ DummyEffect ------------")]
    [SerializeField] string dummy;


    public override void Apply()
    {
        Debug.Log($"using {item.name}");
    }

    public override void Cease()
    {
        Debug.Log($"stop using {item.name}");
    }



}
