using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CheckPoint : MonoBehaviour, Interactable
{
    [SerializeField] Transform respawnPoint;
    [SerializeField] GameObject flame;
    [SerializeField] Sound flameSound;
    [SerializeField] float flameStartDuration = 1f;
    [SerializeField] float flameMaxDuration = 2;
    [SerializeField] float flameStopDuration = 2f;

    List<ParticleSystem> flameParticles;
    Light flameLight;
    float fullBrightness;

    void Start()
    {
        flameParticles = new List<ParticleSystem>(flame.GetComponentsInChildren<ParticleSystem>());
        flameLight = flame.GetComponentInChildren<Light>();
        fullBrightness = flameLight.intensity;
        flameSound?.MakeSource(gameObject);
        flame.SetActive(false);
    }

    public void Interact()
    {
        SaveData.file.Add(SaveData.respawnPosition, transform.position);
        flameSound?.Play(gameObject);
        StartCoroutine(FlameRoutine());
    }

    IEnumerator FlameRoutine()
    {
        enabled = false;
        flame.SetActive(true);
        flameParticles.ForEach(p => p.Play());
        Action<float> incrementLight = timeSoFar => flameLight.intensity = fullBrightness * timeSoFar / flameStartDuration;
        yield return this.EveryFrame(incrementLight, flameStartDuration);

        yield return new WaitForSeconds(flameMaxDuration);

        StartCoroutine(flameSound?.FadeOut(gameObject, flameStopDuration)); // runs async
        flameParticles.ForEach(p => p.Stop());
        Action<float> decrementLight = timeSoFar => flameLight.intensity = fullBrightness * (1 - timeSoFar / flameStopDuration);
        yield return this.EveryFrame(decrementLight, flameStopDuration);
        flame.SetActive(false);
        enabled = true;
    }

}
