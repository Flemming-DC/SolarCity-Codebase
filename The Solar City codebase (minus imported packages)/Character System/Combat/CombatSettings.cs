using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Singleton/CombatSettings")]
public class CombatSettings : ScriptableObjectSingleton<CombatSettings>
{
    public bool makeAttackGizmos;
    public Sound swingSound;
    public Sound hitUndamagableSound;
    public Sound softLandingSound;
    [SerializeField] GameObject sparksPrefab;
    [SerializeField] GameObject dustPrefab;
    [SerializeField] GameObject sphericalDustPrefab;
    public GameObject bareFist;
    public float maxBlockingAngle = 60;
    public float recoilHeight = 0.5f;
    public float recoilSpeed = 0.5f;
    public float sparkDuration = 3;
    public float weaponDecayDuration = 10;
    public float attackGizmoSize = 0.1f;
    public float normalizedToppleFinishedTime_GettingUpFromStomach = 0.77f;
    public float normalizedToppleFinishedTime_GettingUpFromBack = 0.415f;
    public float ragdollToGetupLerpDuration = 0.75f;
    public float toppleDamageMultiplier = 2;
    public AnimationsGivenWeaponClass universalDefaultAnimations;

    public GameObject gizmoParent { get; private set; }

    private void OnEnable()
    {
        gizmoParent = GameObject.Find("Gizmos");
    }

    public void MakeWeaponAudioSources(GameObject weaponObject)
    {
        swingSound?.MakeSource(weaponObject);
        hitUndamagableSound?.MakeSource(weaponObject);
    }

    public void MakeSparks(Vector3 position)
    {
        GameObject sparksInstance = Instantiate(sparksPrefab, position, Quaternion.identity);
        Destroy(sparksInstance, sparkDuration);
    }

    public void MakeDust(Vector3 position, Quaternion rotation, float dustScale)
    {
        GameObject dustInstance = Instantiate(dustPrefab, position, rotation);
        dustInstance.transform.localScale *= dustScale;
        Destroy(dustInstance, sparkDuration);
    }

    public void MakeSphericalDust(Vector3 position, Quaternion rotation, float dustScale)
    {
        GameObject dustInstance = Instantiate(sphericalDustPrefab, position, rotation);
        dustInstance.transform.localScale *= dustScale;
        Destroy(dustInstance, sparkDuration);
    }

}
