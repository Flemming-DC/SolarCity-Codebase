using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rotate : ActionState
{
    Func<Quaternion> GetRotation;
    Transform model;
    ActMachine actMachine;
    float rotationSpeed;

    private void Start()
    {
        model = GetComponentInParent<CharacterReferencer>().Model.transform;
        actMachine = this.GetComponentInSiblings<ActMachine>();
        rotationSpeed = GetComponentInParent<NavMeshAgent>().angularSpeed;
    }

    public override void OnEnter(object GetRotation_)
    {
        GetRotation = (Func<Quaternion>)GetRotation_;
    }

    public override void Tick()
    {
        model.rotation = Quaternion.RotateTowards(
            model.rotation, GetRotation(), rotationSpeed * Time.deltaTime);

        if (Math.Abs(Quaternion.Angle(model.rotation, GetRotation())) > 0.1f)
            return;
        model.rotation = GetRotation();
        actMachine.SetState<Idle>();
    }





}
