using System;
using System.Collections;
using UnityEngine;
using ConditionalField;
using System.Linq;
using Animancer;

public class Ragdoll : MonoBehaviour, IRagdoll
{
    //play the two getup animations in a getup state

    //[SerializeField] bool deactivateInsteadOfDestroy;
    [SerializeField] Sound deathSound;
    [SerializeField] bool createWeaponsInRagdoll = true;
    [ConditionalField(nameof(createWeaponsInRagdoll))] public Transform ragdollLeftHand;
    [ConditionalField(nameof(createWeaponsInRagdoll))] public Transform ragdollRightHand;
    public Transform animatedHips;
    public Transform ragdollHips;
    [SerializeField] float ragdollRestingErrorMargin = 0.1f;

    public bool isDead { get; private set; }
    public bool isToppled { get; set; }
    public Transform character { get; private set; }
    const float minRagdollDuration = 1;
    ScivoloMover mover;
    BodyParts bodyParts;
    Transform animatedModel;
    Transform ragdollModel;
    ActMachine actMachine;
    AnimancerComponent animancer;
    CapsuleCollider capsuleCollider;
    Animator animator;
    Rigidbody characterRigidBody;


    private void Start()
    {
        character = transform.parent;
        characterRigidBody = character.GetComponent<Rigidbody>(true);
        capsuleCollider = character.GetComponent<CapsuleCollider>(true);
        mover = this.GetComponentInSiblings<ScivoloMover>();
        actMachine = this.GetComponentInSiblings<ActMachine>();
        bodyParts = this.GetComponentInSiblings<BodyParts>();
        animator = bodyParts.GetComponent<Animator>();
        animancer = bodyParts.GetComponent<AnimancerComponent>();
        animatedModel = bodyParts.transform.GetChild(0);
        ragdollModel = transform.GetChild(0);
        deathSound?.MakeSource(gameObject);

        ragdollModel.gameObject.SetActive(false);
        if (createWeaponsInRagdoll)
            if (ragdollLeftHand == null || ragdollRightHand == null)
                Debug.LogWarning($"ragdollLeftHand, ragdollRightHand " +
                                 $"shouldn't be null, if you wish to assign weapons to the ragdoll");
        if (transform.childCount != 1 || bodyParts.transform.childCount != 1)
            Debug.LogWarning($"the Model and Ragdoll gameobjects should each have exectly one child. " +
                             $"Namely the animated model and the ragdoll model. However, {transform.parent.name} violates " +
                             $"this rule.");
    }




    //------------ Ragdoll ------------//

    public IEnumerator ToppleRoutine()
    {
        isToppled = true;
        float initialHeight = character.position.y;
        MakeRagdollForToppling();

        //let character follow ragdoll
        transform.SetParent(null);
        float finalTime = Time.time + minRagdollDuration;
        Rigidbody ragdollRigidbody = GetComponentInChildren<Rigidbody>(); // nope, denne linje kan ikke ligge ved start, da GetComponentInChildren kun søger i aktive children
        Func<bool> NotResting = () => ragdollRigidbody.velocity.sqrMagnitude >= ragdollRestingErrorMargin || Time.time < finalTime;
        while (NotResting())
        {
            character.position = ragdollHips.position;
            SnapToGround.TransformToGround(character);
            yield return null;
        }
        transform.SetParent(character);

        // Get data about the first keyframe in the getup animation, so that that getup state can lerp from ragdoll to animation
        animancer.Animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        ClipTransitionAsset.UnShared getupClip = actMachine.GetState<Getup>().GetAnimation(ragdollHips);
        animancer.Play(getupClip, 0);
        yield return null;
        KeyFrameData getupData = new KeyFrameData(animatedHips);
        animancer.Animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;

        // resetting character and entering getup state
        ActivateOtherLogicInCharacter(false);
        CopyCharacterToRagdoll();
        ragdollModel.gameObject.SetActive(false);
        animatedModel.gameObject.SetActive(true);
        ActivateOtherLogicInCharacter(true, false);

        actMachine.SetNextState(typeof(Getup), null, (animatedHips, getupData));
        float finalHeight = character.position.y;
        actMachine.GetState<Landing>().TakeFallingDamage(Mathf.Abs(initialHeight - finalHeight));
    }


    void MakeRagdollForToppling()
    {
        if (createWeaponsInRagdoll)
            CreateWeaponsInRagdoll();
        CopyTransform(ragdollModel, animatedModel, mover.velocity);
        ragdollModel.gameObject.SetActive(true);
        animatedModel.gameObject.SetActive(false);
        actMachine.enabled = false;
        capsuleCollider.enabled = false;
    }

    void CopyCharacterToRagdoll()
    {
        if (createWeaponsInRagdoll)
            CreateWeaponsInRagdoll();
        CopyTransform(animatedHips, ragdollHips, Vector3.zero);

        Vector3 hipsPosition = animatedHips.position;
        character.position = hipsPosition;
        SnapToGround.TransformToGround(character);
        animatedHips.position = hipsPosition;

        characterRigidBody.velocity = Vector3.zero;
        mover.velocity = Vector3.zero;
    }

    void ActivateOtherLogicInCharacter(bool activate, bool includingAnimation = true)
    {
        foreach (Transform child in character)
            if (child != transform && child != character.Find("Actions"))
                child.gameObject.SetActive(activate);
        capsuleCollider.enabled = activate;
        mover.Enabled(activate);
        actMachine.enabled = activate;
        if (includingAnimation)
            animator.enabled = activate;
    }


    //------------ DIE ------------//

    public void Die()
    {
        isDead = true;
        deathSound?.Play(gameObject);
        if (createWeaponsInRagdoll)
            CreateWeaponsInRagdoll();
        CopyTransform(ragdollModel, animatedModel, mover.velocity);
        ReplaceCharacterWithRagdoll();
        
    }
    
    void CreateWeaponsInRagdoll()
    {
        StartCoroutine(CreateWeaponRoutine(bodyParts.leftHand, ragdollLeftHand));
        StartCoroutine(CreateWeaponRoutine(bodyParts.rightHand, ragdollRightHand));
    }

    IEnumerator CreateWeaponRoutine(Transform modelHand, Transform ragdollHand)
    {
        foreach (Transform child in ragdollHand)
        {
            child.SetParent(null);
            Destroy(child.gameObject);
        }
        if (modelHand.childCount == 0)
            yield break;

        GameObject weaponObject = Instantiate(modelHand.GetChild(0).gameObject, ragdollHand, false);
        Destroy(weaponObject.GetComponent<Weapon>());
        foreach (var audio in weaponObject.GetComponents<AudioSource>())
            Destroy(audio);
        if (weaponObject.TryGetComponent(out ShieldRotator shieldRotator))
            shieldRotator.Destroy();

        yield return new WaitForSeconds(CombatSettings.instance.weaponDecayDuration);
        Destroy(weaponObject);

    }

    void CopyTransform(Transform copyInto, Transform copyFrom, Vector3 velocity)
    {
        Transform[] copyIntoChildren = copyInto.GetComponentsInChildren<Transform>();
        Transform[] copyFromChildren = copyFrom.GetComponentsInChildren<Transform>();

        if (copyIntoChildren.Length != copyFromChildren.Length)
        {
            Debug.LogWarning($"Invalid transform copy, they need to match in their transform hierarchies. " +
                             $"copyInto = {copyInto.parent.name}.{copyInto.name} has {copyIntoChildren.Length} children, " +
                             $"but copyFrom = {copyFrom.parent.name}.{copyFrom} has {copyFromChildren.Length} children.");

            string[] copyIntoNames = copyIntoChildren.Select(i => i.name.Replace("(Clone)", "")).ToArray();
            string[] copyFromNames = copyFromChildren.Select(f => f.name.Replace("(Clone)", "")).ToArray();
            copyIntoNames.Where(i => !copyFromNames.Contains(i)).ToArray().Print("Only in copyInto: ");
            copyFromNames.Where(f => !copyIntoNames.Contains(f)).ToArray().Print("Only in copyFrom: ");
            return;
        }

        for (int i = 0; i < copyIntoChildren.Length; i++)
        {
            copyIntoChildren[i].position = copyFromChildren[i].position;
            copyIntoChildren[i].rotation = copyFromChildren[i].rotation;

            Rigidbody rb = copyIntoChildren[i].GetComponent<Rigidbody>();
            if (rb != null)
                rb.velocity = velocity;
        }
    }

    void ReplaceCharacterWithRagdoll()
    {
        transform.SetParent(null);
        Destroy(character.gameObject);
        foreach (var shieldRotator in character.GetComponentsInChildren<ShieldRotator>())
            shieldRotator.Destroy();
        ragdollModel.gameObject.SetActive(true);
    }



}
