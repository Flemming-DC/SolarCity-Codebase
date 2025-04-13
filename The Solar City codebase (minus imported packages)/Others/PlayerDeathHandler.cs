using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] SceneLoader sceneLoader;
    [SerializeField] GameObject sceneVisuals;

    ADamagable playerDamagable;

    void Start()
    {
        playerDamagable = PlayerReferencer.player.GetComponentInChildren<ADamagable>();
        playerDamagable.OnDie += OnPlayerDie;
    }



    private void OnDestroy()
    {
        playerDamagable.OnDie -= OnPlayerDie;
    }


    private void OnPlayerDie()
    {
        InputManager.SetActionMap(InputManager.death);
        //DontDestroyOnLoad(sceneVisuals);
        sceneLoader.LoadCityScene();
    }




}
