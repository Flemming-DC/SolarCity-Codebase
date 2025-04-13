using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShadowToggler : MonoBehaviour
{
    [SerializeField] bool useManualSetup = true; //
    [SerializeField] GameObject[] rendererContainers;
    List<Renderer> renderers = new();

    void Awake()
    {
        foreach (var container in rendererContainers)
            renderers.AddRange(container.GetComponentsInChildren<Renderer>(true));
        if (useManualSetup)
            ToggleShadows(ASettingsCanvas.useCheapGraphics);

        ASettingsCanvas.onToggleCheapGraphics += ToggleShadows;
    }

    void OnDestroy()
    {
        ASettingsCanvas.onToggleCheapGraphics -= ToggleShadows;
    }

    void ToggleShadows(bool useCheapGraphics)
    {
        foreach (var renderer in renderers)
        {
            if (renderer == null)
                continue;
            renderer.shadowCastingMode = useCheapGraphics 
                ? ShadowCastingMode.Off : ShadowCastingMode.On;
            //renderer.receiveShadows = !useCheapGraphics;
        }
    }




}
