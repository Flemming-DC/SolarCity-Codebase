#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Sound))]
public class Sound_Inspector : Editor
{
    static GameObject soundObject;
    string soundObjectName = $"Dummy Sound Player Made By Sound_Inspector";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Sound sound = (Sound)target;

        if (GUILayout.Button("Play"))
        {
            if (soundObject == null)
                soundObject = new GameObject(soundObjectName);

            sound.DestroyImmediateSource(soundObject);
            sound.MakeSource(soundObject);
            sound.Play(soundObject);
        }

        if (GUILayout.Button("CleanUp"))
        {
            if (soundObject != null)
                DestroyImmediate(soundObject);

            while (GameObject.Find(soundObjectName) != null)
                DestroyImmediate(GameObject.Find(soundObjectName));

            if (GameObject.Find(soundObjectName) != null)
                Debug.LogWarning($"There is a GameObject called {soundObjectName}, which didn't get destroyed by the CleanUp command.");
        }

    }


}
#endif