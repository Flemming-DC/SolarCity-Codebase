using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "ScriptableObject/Sound")]
public class Sound : ScriptableObject
{
    [SerializeField] AudioClip audioClip;
    [SerializeField] AudioMixerGroup audioMixerGroup;
    [SerializeField] bool playOnAwake = false;
    [SerializeField] bool isLooping = false;
    [Range(0, 1)] [SerializeField] float volume = 0.5f;
    [Range(0, 3)] [SerializeField] float pitch = 1;
    [Range(0, 1)] [SerializeField] float spatialBlend = 1;
    [SerializeField] float minDistance = 4;
    [SerializeField] float maxDistance = 100;

    
    Dictionary<GameObject, AudioSource> audioSources = new Dictionary<GameObject, AudioSource>();
    float lastTime;

    private void OnEnable()
    {
        lastTime = -1000;
    }

    public AudioSource MakeSource(GameObject gameObject)
    {
        if (audioSources.ContainsKey(gameObject))
            return audioSources[gameObject];

        AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        InitializeAudioSource(audioSource);
        audioSources.Add(gameObject, audioSource);
        if (playOnAwake)
            Play(gameObject);
        return audioSource;
    }

    public void DestroyImmediateSource(GameObject gameObject)
    {
        if (!audioSources.ContainsKey(gameObject))
            return;
        DestroyImmediate(audioSources[gameObject]);
        audioSources.Remove(gameObject);
    }

    void InitializeAudioSource(AudioSource audioSource)
    {
        audioSource.clip = audioClip;
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.playOnAwake = playOnAwake;
        audioSource.loop = isLooping;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.spatialBlend = spatialBlend;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;

        if (pitch == 0)
            Debug.LogWarning($"{name} has zero pitch, so it cannot be heard");
        if (volume == 0)
            Debug.LogWarning($"{name} has zero volume, so it cannot be heard");
        if (minDistance >= maxDistance)
            Debug.LogWarning($"{name} has minDistance >= maxDistance, which seems wrong");
    }

    public void Play(GameObject gameObject, bool interrupt = true, float? lag = null, bool ignoreWarning = false)
    {
        if (!audioSources.ContainsKey(gameObject))
        {
            Debug.LogWarning($"Couldn't find {gameObject} among the GameObjects with an audioSource created by {this}");
            return;
        }
        if (!interrupt)
        {
            float duration = lag == null
                             ? audioClip.length / audioSources[gameObject].pitch
                             : (float)lag;
            if (Time.time - lastTime < duration)
                return;
        }

        lastTime = Time.time;
        if (gameObject.activeInHierarchy)
            audioSources[gameObject].Play();
        else if (!ignoreWarning)
            Debug.LogWarning($"trying to play audio on {gameObject.transform.Path()}, even though it isn't activeInHierarchy");
    }


    public IEnumerator FadeOut(GameObject gameObject, float duration)
    {
        if (!audioSources.ContainsKey(gameObject))
        {
            Debug.LogWarning($"Couldn't find {gameObject} among the GameObjects with an audioSource created by {this}");
            yield break;
        }
        float timeSoFar = 0;
        while (timeSoFar < duration)
        {
            audioSources[gameObject].volume = volume * (1 - timeSoFar / duration);
            timeSoFar += Time.deltaTime;
            yield return null;
        }
        audioSources[gameObject].Stop();
        audioSources[gameObject].volume = volume;
    }

    public void Stop(GameObject gameObject)
    {
        if (!audioSources.ContainsKey(gameObject))
        {
            Debug.LogWarning($"Couldn't find {gameObject} among the GameObjects with an audioSource created by {this}");
            return;
        }
        audioSources[gameObject].Stop();
    }

    /*
    public void StopOnEveryObject()
    {
        foreach (var audio in audioSources.Values)
            audio.Stop();
    }

    public AudioClip GetClip() => audioClip;
    */
}
