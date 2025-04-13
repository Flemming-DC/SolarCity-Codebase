#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class PlayShortCut
{
    
    [MenuItem("Custom/Run _F5")] // shortcut key F5 to Play (and exit playmode also)
    static void PlayGame()
    {
        if (!Application.isPlaying)
        {
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "", false);
        }
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }

}
#endif
