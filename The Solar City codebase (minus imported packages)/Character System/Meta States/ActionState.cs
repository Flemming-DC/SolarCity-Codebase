using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionState : State
{
    public bool inVoluntary { get; protected set; }
    public bool interruptable { get; protected set; }

    public override void OnEnter(object dummy) { }
    public override void Tick() { }
    public override void OnExit() { }
}
