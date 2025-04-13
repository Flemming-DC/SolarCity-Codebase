using UnityEngine;

public class Throw : RootMotionAction
{
    // evt. allow early exit from throw (fx make it interruptible after launch)
    [SerializeField] float leaveHandNormalizedTime = 0.496f;
    [SerializeField] float throwSpeed = 12;
    [SerializeField] float throwAngle = 15;
    [SerializeField] Transform model;
    [SerializeField] GameObject bombPrefab;

    TwoHandingIK twoHandingIK;
    GameObject bombInstance;
    AWeaponEquiper equipper;

    protected override void Start()
    {
        twoHandingIK = this.GetComponentInSiblings<TwoHandingIK>();
        equipper = this.GetComponentInSiblings<WeaponHandler>().equipper;
        maxTime = 10;
        base.Start();
    }


    public override void OnEnter(object bomb)
    {
        base.OnEnter(null);
        twoHandingIK.activationOverriders.Add(this);
        GameObject bombObj = (GameObject)bomb;

        if (bombObj.IsPrefab())
        {
            bombInstance = equipper.ReplaceWeapon(Hand.right, bombPrefab);
            bombInstance.GetComponent<Rigidbody>().useGravity = false;
            bombInstance.GetComponent<Collider>().enabled = false;
        }
        else
            bombInstance = bombObj;
    }

    public override void Tick()
    {
        base.Tick();
        bool hasExploded = bombInstance == null;
        if (hasExploded)
            return;

        bool hasLaunched = bombInstance.transform.parent == null;
        bool shallLaunch = actionAnimation.State.NormalizedTime > leaveHandNormalizedTime;
        if (shallLaunch && !hasLaunched)
        {
            bombInstance.transform.parent = null;
            bombInstance.GetComponent<Collider>().enabled = true;
            Rigidbody rb = bombInstance.GetComponent<Rigidbody>();
            rb.velocity = throwSpeed * (  Mathf.Cos(throwAngle * Mathf.Deg2Rad) * model.forward 
                                        + Mathf.Sin(throwAngle * Mathf.Deg2Rad) * model.up);
            rb.useGravity = true;
        }

    }

    public override void OnExit()
    {
        base.OnExit();
        equipper.BringBackWeapons();
        twoHandingIK.activationOverriders.Remove(this);
        if (bombInstance != null)
            if (bombInstance.transform.parent != null)
                bombInstance.transform.parent = null;
    }









}
