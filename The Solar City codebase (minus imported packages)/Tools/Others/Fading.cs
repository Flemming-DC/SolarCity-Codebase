using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fading : MonoBehaviour
{
    [SerializeField] float fadingDuration = 0.5f;

    Graphic[] graphics;

    private void Start()
    {
        graphics = GetComponentsInChildren<Graphic>();
    }

    public void Fade(bool fadeInNotOut, Action onFinished = null)
    {
        gameObject.SetActive(true);
        if (fadeInNotOut)
            StartCoroutine(FadeIn(onFinished));
        else
            StartCoroutine(FadeOut(onFinished));
    }


    IEnumerator FadeIn(Action onFinished)
    {
        yield return this.EveryFrame(
            timeSoFar => SetFading(timeSoFar / fadingDuration),
            fadingDuration);
        onFinished();
    }

    IEnumerator FadeOut(Action onFinished)
    {
        yield return this.EveryFrame(
            timeSoFar => SetFading(1 - timeSoFar / fadingDuration),
            fadingDuration);

        gameObject.SetActive(false);
        onFinished();
    }


    void SetFading(float fadingValue)
    {
        foreach (var graphic in graphics)
            graphic.color = graphic.color.With(a: fadingValue);
    }

}
