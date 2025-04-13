using System;
using System.Collections;
using UnityEngine;
using Animancer;

public class BowShot : ActionState
{
    // evt. bring back weapons
    [SerializeField] float drawArrowAnimationSpeed = 1;
    [SerializeField] float ShootArrowNormalizedTime = 0.496f;
    [SerializeField] float angleCorrection = 90;
    [SerializeField] float shootHeightCorrection = 0;
    [SerializeField] Transform model;
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] ClipTransition shootingAnimation;

    Vector3 shootDirection;
    Transform target;
    GameObject arrowInstance;
    AnimancerComponent animancer;
    Quaternion currentRotation;
    float rotationSpeed;
    bool rotationIsDone;
    bool startedDrawing;
    bool hasShot;

    void Start()
    {
        rotationSpeed = GetComponent<AIMotion>().rotationSpeed;
        animancer = this.GetComponentInSiblings<AnimancerComponent>();
        //maxTime = 10; // the archer can in principle rotate forever, if you circle around him.

        if (arrowPrefab == null)
            Debug.LogError($"arrowPrefab is null");
    }



    public override void OnEnter(object target_)
    {
        if (target_.GetType() != typeof(Transform))
            Debug.LogError($"target_ = {target_}, which is not a Transform");
        target = (Transform)target_;
        currentRotation = model.rotation;
        rotationIsDone = false;
        startedDrawing = false;
        hasShot = false;
    }

    public override void Tick()
    {
        if (transform == null) // is this really necessary?
            return;
        if (target == null)
            this.GetComponentInSiblings<ActMachine>().SetState<Idle>();
        if (!rotationIsDone)
            Rotate();
        else if (!startedDrawing)
            Draw();
        else if (!hasShot && shootingAnimation.State.NormalizedTime > ShootArrowNormalizedTime)
            Shoot();
        else if (animancer.IsFinished())
            this.GetComponentInSiblings<ActMachine>().SetNextState(typeof(Idle));

    }

    public override void OnExit()
    {
        if (arrowInstance == null)
            return;
        if (arrowInstance.transform.parent == null)
            return;
        arrowInstance.transform.parent = null;
        Destroy(arrowInstance);
    }



    void Rotate()
    {
        Vector3 displacement = (target.position - transform.position).With(y: 0);
        // we must adjust for the fact that the animation is off by angleCorrection degrees
        displacement = Quaternion.AngleAxis(angleCorrection, Vector3.up) * displacement; 
        Quaternion targetRotation = Quaternion.LookRotation(displacement, Vector3.up);
        currentRotation = Quaternion.RotateTowards(
            currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        model.rotation = currentRotation;

        rotationIsDone = Quaternion.Angle(model.rotation, targetRotation) <= 1f;
    }

    void Draw()
    {
        BodyParts bodyparts = this.GetComponentInSiblings<BodyParts>();
        animancer.Play(shootingAnimation);
        animancer.States.Current.Speed = drawArrowAnimationSpeed;

        arrowInstance = Instantiate(arrowPrefab, bodyparts.rightHand);
        arrowInstance.transform.position = bodyparts.rightHand.position;
        arrowInstance.transform.rotation = bodyparts.rightHand.rotation;

        startedDrawing = true;
        shootDirection = (target.position + shootHeightCorrection * Vector3.up - transform.position).normalized;
    }

    void Shoot()
    {
        var shooter = this.GetComponentInSiblings<CharacterDamagable>();
        arrowInstance.GetComponent<Arrow>().Fire(shooter, shootDirection);
        animancer.States.Current.Speed = 1;
        hasShot = true;
    }




}
