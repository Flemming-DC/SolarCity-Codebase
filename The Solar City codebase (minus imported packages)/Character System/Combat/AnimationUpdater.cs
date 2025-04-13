using System;
using UnityEngine;

public class AnimationUpdater : MonoBehaviour
{
    [SerializeField] AnimationsGivenWeaponClass defaultAnimations;

    public AnimationsGivenWeaponClass animations { get; private set; }
    public event Action<AnimationsGivenWeaponClass> OnAnimationsChanged;

    private void Awake()
    {
        animations = ScriptableObject.CreateInstance<AnimationsGivenWeaponClass>();
        SetAnimations(defaultAnimations);
    }

    public void SetAnimations(AnimationsGivenWeaponClass animations_)
    {
        animations.idle = animations_.idle;
        animations.hurt = animations_.hurt;
        animations.veryHurt = animations_.veryHurt;
        animations.run = animations_.run;
        animations.sprint = animations_.sprint;
        animations.strafingMixer = animations_.strafingMixer;

        OnAnimationsChanged?.Invoke(animations_);
    }

}
