using System;
using UnityEngine;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField] CollisionEvent WinBox;

    void Awake()
    {
        Time.timeScale = 1;
        WinBox.onTriggerEnter.AddListener(_ => Win());
        
    }

    void OnDestroy()
    {
        Time.timeScale = 1;
        WinBox.onTriggerEnter.RemoveListener(_ => Win());
    }


    void Win()
    {
        Action openWindow = () => GetComponent<AWindow>().Open(false);
        GetComponent<Fading>().Fade(true, openWindow);
        Time.timeScale = 0;
        InputManager.gameplay.Disable();
    }





}
