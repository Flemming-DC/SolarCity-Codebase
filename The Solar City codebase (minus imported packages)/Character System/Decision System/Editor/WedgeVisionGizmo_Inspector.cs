#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WedgeVisionGizmo))]
public class WedgeVisionGizmo_Inspector : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        WedgeVisionGizmo visionGizmo = (WedgeVisionGizmo)target;
        visionGizmo.Start();
        visionGizmo.GetComponent<Vision>().Scan();
        

    }



}

#endif
