using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;


#if UNITY_EDITOR
[CustomEditor(typeof(DeactivateDetails))]
public class DeactivateDetails_Inspector : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var deactivateDetails = (DeactivateDetails)target;

        if (GUILayout.Button("toggle activation"))
            deactivateDetails.ToggleActivation();

    }



}
#endif
