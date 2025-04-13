using System;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    public float? maxTime { get; protected set; } = null;

    public abstract void OnEnter(object input);
    public abstract void Tick();
    public abstract void OnExit();
}
