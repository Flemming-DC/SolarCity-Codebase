using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GateOpeningStatue : MonoBehaviour, Interactable
{
    [SerializeField] GameObject gateRisingCamera;
    [SerializeField] GameObject defaultCamera;
    [SerializeField] Gate gate;
    [SerializeField] float cameraPause = 0.5f;
    [SerializeField] float fullRange = 3f;
    [SerializeField] float fullBrightness = 3f;
    [SerializeField] float glowDuration = 3.5f;
    [SerializeField] Light[] lights = new Light[2];

    float initialRange;
    float initialBrightness;

    void Start()
    {
        initialBrightness = lights[0].intensity;
        initialRange = lights[0].range;
        foreach (var light in lights)
            if (light.intensity != initialBrightness || light.range != initialRange)
                Debug.LogWarning(
                    $"Each of the lights on the {name}.{nameof(GateOpeningStatue)}, should have the same brightness " +
                    $"and range, but that is not the case for {lights[0].name} and {light.name}");
        Load();
    }

    public void Interact()
    {
        StartCoroutine(InteractionRoutine());
    }


    IEnumerator InteractionRoutine()
    {
        yield return StartCoroutine(GlowRoutine());

        yield return new WaitForSecondsRealtime(0.8f * cameraPause);
        gateRisingCamera.SetActive(true);
        defaultCamera.SetActive(false);
        InputManager.inputAsset.Disable();
        yield return new WaitForSecondsRealtime(0.2f * cameraPause);

        yield return StartCoroutine(gate.OpenRoutine());

        yield return new WaitForSecondsRealtime(cameraPause);
        defaultCamera.SetActive(true);
        gateRisingCamera.SetActive(false);
        InputManager.SetActionMap(InputManager.gameplay);
    }

    IEnumerator GlowRoutine()
    {
        enabled = false;
        SaveData.file.Add($"{transform.Path()}.GateOpeningStatue.hasInteracted", true);
        float halfDuration = 0.5f * glowDuration;

        yield return this.EveryFrame(timeSoFar =>
        {
            foreach (var light in lights)
            {
                light.intensity = Mathf.Lerp(initialBrightness, fullBrightness, timeSoFar / halfDuration);
                light.range = Mathf.Lerp(initialRange, fullRange, timeSoFar / halfDuration);
            }
        }, halfDuration);

        yield return this.EveryFrame(timeSoFar =>
        {
            foreach (var light in lights)
            {
                light.intensity = Mathf.Lerp(fullBrightness, initialBrightness, timeSoFar / halfDuration);
                light.range = Mathf.Lerp(fullRange, initialRange, timeSoFar / halfDuration);
            }
        }, halfDuration);
    }


    void Load()
    {
        if (!SaveData.loaded)
            return;
        if (!SaveData.file.GetBool($"{transform.Path()}.GateOpeningStatue.hasInteracted"))
            return;

        gate.SetOpenInstantly(true);
        enabled = false;
    }

}
