using UnityEngine;
using Animancer;

[CreateAssetMenu(menuName = "ScriptableObject/AnimationsGivenWeaponClass")]
public class AnimationsGivenWeaponClass : ScriptableObject
{
    [Header("Passive")]
    public ClipTransition idle;
    public ClipTransition hurt;
    public ClipTransition veryHurt;
    [Header("Movement")]
    public ClipTransition run; 
    public ClipTransition sprint;
    public MixerTransition2D strafingMixer;


}

