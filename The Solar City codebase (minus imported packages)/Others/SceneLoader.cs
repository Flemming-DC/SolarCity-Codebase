using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] string menuScene;
    [SerializeField] string cityScene;
    [SerializeField] Graphic loadingBackGround;
    [SerializeField] List<Graphic> DeathTexts;
    [SerializeField] float fadingDuration = 0.5f;
    [SerializeField] AnimationCurve fadingCurve;

    List<Graphic> graphics = new List<Graphic>();
    static bool dead;

    void Start()
    {
        graphics.Add(loadingBackGround);
        graphics.AddRange(DeathTexts);
        StartCoroutine(FadeLoadingToScene());
    }



    public void LoadCityScene()
    {
        StartCoroutine(LoadRoutine(cityScene));
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadRoutine(menuScene));
    }

    public void RestartGame()
    {
        SaveData.file.Dispose();
        StartCoroutine(LoadRoutine(cityScene));
    }

    public void Quit()
    {
        print("QUIT");
        Application.Quit();
    }


    IEnumerator LoadRoutine(string sceneName)
    {
        yield return StartCoroutine(FadeSceneToLoading());
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeSceneToLoading()
    {
        // We save playerDamagable.dead in a static variable to preserve it during scene loading
        var player = PlayerReferencer.player;
        dead = player != null ? player.GetComponentInChildren<ADamagable>().dead : false;
        
        loadingBackGround.gameObject.SetActive(true);
        DeathTexts.ForEach(dt => dt.gameObject.SetActive(dead));
        yield return this.EveryFrame(
            timeSoFar => SetFading(timeSoFar / fadingDuration), 
            fadingDuration);
    }

    // this could be simplified with fading.Fade()
    IEnumerator FadeLoadingToScene()
    {
        loadingBackGround.gameObject.SetActive(true);
        DeathTexts.ForEach(dt => dt.gameObject.SetActive(dead));
        float fadingParameter = 1;
        while (fadingParameter >= 0)
        {
            SetFading(fadingParameter);

            fadingParameter -= Time.deltaTime / fadingDuration;
            yield return null;
        }
        SetFading(0);
        yield return null;
        loadingBackGround.gameObject.SetActive(false);
        DeathTexts.ForEach(dt => dt.gameObject.SetActive(false));
        dead = false;
    }


    void SetFading(float fadingParameter)
    {
        float fadingValue = fadingCurve.Evaluate(fadingParameter);
        foreach (var graphic in graphics)
            graphic.color = graphic.color.With(a: fadingValue);
    }


}
