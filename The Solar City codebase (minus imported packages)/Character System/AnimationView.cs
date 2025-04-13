using System.Linq;
using System;
using UnityEngine;
using Animancer;

public class AnimationView : MonoBehaviour
{
    [SerializeField] Attack attack;
    [SerializeField] ClipTransition animation_;
    [Range(0, 1)] public float normalizedTime;



    public void Reset()
    {
        Transform[] modelBones = transform.GetChild(0).GetComponentsInChildren<Transform>();
        Transform[] ragdollBones = this.GetComponentInSiblings<Ragdoll>()
            .transform.GetChild(0).GetComponentsInChildren<Transform>()
            .Where(t => !t.TryGetComponent(out SkinnedMeshRenderer _))
            .ToArray();

        var defaultPositions = ragdollBones.ToDictionary(b => b.name, b => b.localPosition);
        var defaultRotations = ragdollBones.ToDictionary(b => b.name, b => b.localRotation);

        foreach (var bone in modelBones)
        {
            if (!defaultPositions.ContainsKey(bone.name))
                continue;
            bone.localPosition = defaultPositions[bone.name];
            bone.localRotation = defaultRotations[bone.name];
        }
        animation_ = null;
        attack = null;
    }

    void OnValidate()
    {
        AnimancerComponent animancer = GetComponent<AnimancerComponent>();
        if (attack != null)
            animation_ = attack.animation;
        if (animation_ == null)
            return;
        animation_.Clip.EditModeSampleAnimation(animancer.Animator, normalizedTime);
    }


    private void OnDrawGizmosSelected()
    {
        if (attack == null)
            return;

        // nb: there is no gizmo for left attack range or for max/min attack range
        // float rightAttackRange = this.GetComponentInSiblings<BodyParts>().rightWeapon.GetComponent<Weapon>().range;

        var bodyParts = GetComponent<BodyParts>();
        float rightRange = 0;
        if (bodyParts.rightWeapon != null)
            if (bodyParts.rightWeapon != null)
                if (bodyParts.rightWeapon.TryGetComponent(out Weapon rightWeapon))
                    rightRange = rightWeapon.range;
        float leftRange = 0;
        if (bodyParts.leftWeapon != null)
            if (bodyParts.leftWeapon != null)
                if (bodyParts.leftWeapon.TryGetComponent(out Weapon leftWeapon))
                    leftRange = leftWeapon.range;
        float maxRange = MathF.Max(leftRange, rightRange);

        Gizmos.color = Color.red;
        Vector3 forwardDirection = transform.forward;
        Vector3 startPosition = transform.position + attack.sweepHeight * Vector3.up;
        float currentAngle = 0.5f * attack.sweepSizeAngle + attack.sweepAngleOffset;
        float deltaAngle = attack.sweepSizeAngle / (attack.hitDetectionRayCount - 1);

        for (int i = 0; i < attack.hitDetectionRayCount; i++)
        {
            Vector3 hitDirection = Quaternion.AngleAxis(attack.sweepOrientationAngle, forwardDirection)
                * Quaternion.Euler(0, currentAngle, 0)
                * forwardDirection;
            Vector3 endPosition = startPosition + hitDirection * maxRange;
            Gizmos.DrawLine(startPosition, endPosition);
            currentAngle -= deltaAngle;
        }

    }


}
