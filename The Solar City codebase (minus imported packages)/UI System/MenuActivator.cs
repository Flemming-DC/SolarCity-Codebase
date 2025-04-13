using System;
using System.Collections;
using UnityEngine;

public class MenuActivator : MonoBehaviour
{
    [SerializeField] Window pauseWindow;

    public static Window[] allWindows;
    public static Action<bool> onTogglePause; //arg = paused

    IEnumerator Start()
    {
        bool pausedAtStart = pauseWindow != null && pauseWindow.gameObject.activeSelf;
        StartCoroutine(SetupMenusRoutine(pausedAtStart));

        if (pauseWindow == null)
            yield break;

        InputManager.gameplay.OpenMenus.performed += _ => Pause();
        InputManager.UI.CloseMenus.performed += _ => UnPause();

    }

    private void OnDestroy()
    {
        allWindows = null;
    }


    IEnumerator SetupMenusRoutine(bool pausedAtStart)
    {
        allWindows = GetComponentsInChildren<Window>(true);

        foreach (var window in allWindows)
            window.Open(false);
        yield return null;
        foreach (var window in allWindows)
            window.Close(false);

        yield return null;
        if (pausedAtStart)
            pauseWindow?.Open(false);
    }



    void Pause()
    {
        foreach (var window in allWindows)
            window.Close(false);
        pauseWindow.Open(false);
        InputManager.SetActionMap(InputManager.UI);
        onTogglePause?.Invoke(true);
    }

    void UnPause()
    {
        foreach (var window in allWindows)
            window.Close(false);
        InputManager.SetActionMap(InputManager.gameplay);
        onTogglePause?.Invoke(false);
    }


}
