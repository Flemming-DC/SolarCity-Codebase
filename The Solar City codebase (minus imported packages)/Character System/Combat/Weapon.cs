using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;
using ConditionalField;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class Weapon : MonoBehaviour, IWeapon
{
    [SerializeField] bool isTwoHanded;
    public bool hasFallingAttack = true;
    public float range = 1.5f;
    [ConditionalField(nameof(isTwoHanded))] public Transform leftHandGrip;
    //use hitUndamagable when striking armour or getting parried. Possibly use special sound for breaking shields.
    [SerializeField] Sound hitSound; 
    [SerializeField] GameObject bloodPrefab;
    public Stat bloodScale = 2f;
    [SerializeField] Transform expectedHitPoint;
    public AnimationsGivenWeaponClass animations;
    [Header("---------------- Attacks ----------------")]
    [SerializeField] List<Attack> leftHandAttack_1;
    [SerializeField] Attack leftHandAttack_2;
    [SerializeField] List<Attack> rightHandAttack_1;
    [SerializeField] Attack rightHandAttack_2;

    public Attack attack { get; set; }
    public Hand hand { get; set; }
    public Transform owner { get; private set; }
    public Vector3 forwardDirection { private get; set; }
    public Transform lastHitTarget { get; private set; }
    AnimancerComponent animancer;
    List<GameObject> gizmos = new List<GameObject>();
    TwoHandingIK twoHandingIK;

    private void Start()
    {
        owner = GetOwnerAtStart();
        animancer = owner.GetComponentInDirectChildren<AnimancerComponent>();
        twoHandingIK = owner.GetComponentInDirectChildren<TwoHandingIK>();
        owner.GetComponentInDirectChildren<WeaponHandler>().UpdateIK();
        CombatSettings.instance.MakeWeaponAudioSources(gameObject);
        hitSound?.MakeSource(gameObject);
    }

    void OnEnable()
    {
        if (!gameObject.activeSelf)
            return;
        if (twoHandingIK == null)
            return;
        if (!twoHandingIK.activationOverriders.Contains(this))
            return;
        twoHandingIK.activationOverriders.Remove(this);
    }

    void OnDisable()
    {
        if (gameObject.activeSelf)
            return;
        twoHandingIK.activationOverriders.Add(this);
    }
    
    private void OnDestroy()
    {
        StopAllCoroutines();
        Invoke(nameof(ReplaceNullWithBareFist), Time.deltaTime);
    }


    public IEnumerator DetectHits()
    {
        if(CombatSettings.instance.makeAttackGizmos)
            UpdateGizmos();
        lastHitTarget = null;
        RaycastHit hit = default(RaycastHit);

        foreach (Vector3 hitDirection in GetHitDirections())
        {
            bool hitSomething = HitSomething(hitDirection, attack.sweepHeight, out RaycastHit approximateHit);
            //if (!hitSomething)
            //    hitSomething = HitSomething(hitDirection, 0.5f * attack.sweepHeight, out approximateHit);

            if (!hitSomething)
                continue;
            yield return new WaitUntil(() => attack.IsPassedHittingTime() || HasHit(approximateHit.transform, out hit));

            if (attack.IsPassedHittingTime())
                hit = approximateHit;
            if (hit.transform == null) // this check fixes a bug that comes from Destroy != DestroyImmidiate
                continue;

            if (hit.transform.TryGetComponentInCharacter(out Damagable damagable))
            {
                DealDamage(hit.transform, damagable, out bool stopSearchingForHits, hit);
                if (stopSearchingForHits)
                    yield break;
            }
            else if(hit.transform.IsInLayerMask(LayerData.instance.terrainMask))
            {
                Recoil(hit);
                yield break;
            }
        }
    }

    void UpdateGizmos()
    {
        DestroyGizmos();
        bool itsTheFirstHitDirection = true;

        foreach (Vector3 hitDirection in GetHitDirections())
        {
            Vector3 endPosition = owner.position + attack.sweepHeight * Vector3.up + hitDirection * range;
            Color? color = (itsTheFirstHitDirection) ? (Color?)Color.green : null;
            MakeGizmo(endPosition, color);
            itsTheFirstHitDirection = false;
        }
    }

    GameObject MakeGizmo(Vector3 position, Color? color = null)
    {
        GameObject gizmo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        gizmo.transform.position = position;
        gizmo.transform.localScale *= CombatSettings.instance.attackGizmoSize;
        gizmo.transform.SetParent(CombatSettings.instance.gizmoParent.transform);
        Destroy(gizmo.GetComponent<Collider>());
        gizmos.Add(gizmo);
        if (color != null)
            gizmo.GetComponent<MeshRenderer>().material.SetColor("_Color", (Color)color);

        return gizmo;
    }

    bool HitSomething(Vector3 hitDirection, float height, out RaycastHit approximateHit)
    {
        return Physics.Raycast(
            owner.position + height * Vector3.up,
            hitDirection,
            out approximateHit,
            range,
            LayerData.instance.hitableMask);
    }

    Vector3[] GetHitDirections()
    {
        Vector3[] hitDirections = new Vector3[attack.hitDetectionRayCount];

        float currentAngle = 0.5f * attack.sweepSizeAngle + attack.sweepAngleOffset;
        float deltaAngle = attack.sweepSizeAngle / (attack.hitDetectionRayCount - 1);
        for (int i = 0; i < hitDirections.Length; i++)
        {
            hitDirections[i] = Quaternion.Euler(0, currentAngle, 0) * forwardDirection;
            hitDirections[i] = Quaternion.AngleAxis(attack.sweepOrientationAngle, forwardDirection) * hitDirections[i];
            currentAngle -= deltaAngle;
        }

        return hitDirections;
    }

    public void DealDamage(Transform target, Damagable damagable, out bool stopSearchingForHits, RaycastHit hit = default(RaycastHit))
    {
        stopSearchingForHits = false;
        if (lastHitTarget == target)
            return;
        if (hit.Equals(default(RaycastHit)))
        {
            hit.point = expectedHitPoint.position;
            Vector3 raycastOrigin = owner.position + attack.sweepHeight * Vector3.up;
            hit.normal = -(expectedHitPoint.position - raycastOrigin).normalized;
        }

        Damagable.Reaction reaction = damagable.TakeDamage(this, hit);

        switch (reaction)
        {
            case Damagable.Reaction.miss:
                break;
            case Damagable.Reaction.block:
                lastHitTarget = target;
                if (attack.blockable)
                    stopSearchingForHits = true;
                else
                    hitSound?.Play(gameObject, ignoreWarning: true);
                break;
            case Damagable.Reaction.hurt:
                lastHitTarget = target;
                if (!attack.canHitMultipleTargets)
                    stopSearchingForHits = true;
                hitSound?.Play(gameObject, ignoreWarning: true);
                if (bloodPrefab != null && damagable is CharacterDamagable)
                {
                    GameObject bloodInstance = Instantiate(
                        bloodPrefab,
                        hit.point,
                        Quaternion.LookRotation(Quaternion.Euler(0, -90, 0) * hit.normal));
                    bloodInstance.transform.localScale *= bloodScale;
                }
                break;
        }
    }


    public virtual void Recoil(RaycastHit hit, bool blocked = false)
    {
        if (!blocked)
        {
            if (expectedHitPoint.position.y <= owner.position.y + CombatSettings.instance.recoilHeight)
                return;
            if (attack.unRecoilable)
                return;
        }

        CombatSettings.instance.MakeSparks(hit.point);
        CombatSettings.instance.hitUndamagableSound?.Play(
            gameObject, false, animancer.States.Current.NormalizedTime, ignoreWarning: true);
        animancer.States.Current.Speed = -CombatSettings.instance.recoilSpeed;
    }

    bool HasHit(Transform target, out RaycastHit hit)
    {
        hit = default(RaycastHit);
        if (owner == null)
            return true;
        if (expectedHitPoint == null)
            return true;
        Vector3 raycastOrigin = owner.position + attack.sweepHeight * Vector3.up;
        bool hasHit = Physics.Raycast(raycastOrigin,
                                      expectedHitPoint.position - raycastOrigin, 
                                      out hit, 
                                      range,
                                      LayerData.instance.hitableMask);

        if (hit.transform != target && target != null)
            return false;
        if (hasHit && CombatSettings.instance.makeAttackGizmos)
            MakeGizmo(hit.point, Color.yellow);

        return hasHit;
    }

    public bool IsTwoHanded() => isTwoHanded;

    public (Attack, List<Attack>) GetAttack(Hand hand, AttackType attackType)
    {
        if (hand == Hand.left)
        {
            if (attackType == AttackType.firstAttack)
                return (leftHandAttack_1[0], leftHandAttack_1);
            else if (attackType == AttackType.secondAttack)
                return (leftHandAttack_2, new List<Attack>());
        }
        else if (hand == Hand.right)
        {
            if (attackType == AttackType.firstAttack)
                return (rightHandAttack_1[0], rightHandAttack_1);
            else if (attackType == AttackType.secondAttack)
                return (rightHandAttack_2, new List<Attack>());
        }

        Debug.LogWarning($"Didn't find hand = {hand} and attackType = {attackType}." +
                         $"returning rightHandAttack_1 by default.");
        return (rightHandAttack_1[0], rightHandAttack_1);
    }

    public void DestroyGizmos()
    {
        foreach (var gizmo in gizmos)
            Destroy(gizmo);
        gizmos.Clear();
    }

    public Transform GetOwnerAtStart()
    {
        Transform ancestor = transform;
        while (ancestor.gameObject.layer != LayerMask.NameToLayer("Character"))
            ancestor = ancestor.parent;

        return ancestor;
    }

    void ReplaceNullWithBareFist()
    {
        if (owner == null)
            return;
        WeaponHandler weaponHandler = owner.GetComponentInDirectChildren<WeaponHandler>();
        if (weaponHandler == null)
            return;
        weaponHandler.ReplaceNullWithBareFist();
    }


    public AttackDisplayData[] GetAttackDisplayData() => new AttackDisplayData[]
        {
            rightHandAttack_1[0].GetAttackDisplayData(true),
            rightHandAttack_2.GetAttackDisplayData(false),
            //leftHandAttack_1[0].GetAttackDisplayData(false),
            //leftHandAttack_2.GetAttackDisplayData(false),
        };

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        bool isInPrefabMode = (PrefabStageUtility.GetCurrentPrefabStage() != null);
        if (isInPrefabMode)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetOwnerAtStart().position, range);
    }
    #endif

}
