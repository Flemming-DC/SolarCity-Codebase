#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Animancer;
using Animancer.Editor;

[CustomEditor(typeof(AnimationView))]
public class AnimationView_Inspector : Editor
{


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Reset Animation"))
        {
            AnimationView animationView = (AnimationView)target;
            animationView.Reset();
        }

    }


}
#endif
