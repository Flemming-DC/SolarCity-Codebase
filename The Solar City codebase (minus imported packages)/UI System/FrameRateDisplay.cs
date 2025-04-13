using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrameRateDisplay : MonoBehaviour
{
    [SerializeField] float smoothingTime = 1;
    [SerializeField] TMP_Text fpsText;

    float smoothedFPS;
    float updateParameter;

    private void Start()
    {
        smoothedFPS = Time.deltaTime;
        updateParameter = 1 - Mathf.Exp(-1f / smoothingTime);
    }

    private void Update()
    {
        smoothedFPS = updateParameter * (1f / Time.deltaTime) + (1 - updateParameter) * smoothedFPS;
        
        fpsText.text = $" FPS = {Mathf.Round(smoothedFPS)}";
    }



}
