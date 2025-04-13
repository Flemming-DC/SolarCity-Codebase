using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEditor;
using TMPro;

#if UNITY_EDITOR
[CustomEditor(typeof(HintReferences))]
public class HintReferencer_Inspector : Editor
{
    int index;
    static string[] actionNames;
    static List<InputAction> actions = new List<InputAction>();
    InputAsset inputAsset;

    public override void OnInspectorGUI()
    {
        HintReferences references = (HintReferences)target;
        if (actionNames == null)
            Setup();

        base.OnInspectorGUI();
        DisplayChooseItemEffect(references);

        foreach (var image in references.GetComponentsInChildren<Image>(true))
            EditorUtility.SetDirty(image);
        foreach (var text in references.GetComponentsInChildren<TMP_Text>(true))
            EditorUtility.SetDirty(text);
    }


    void DisplayChooseItemEffect(HintReferences references)
    {
        GUILayout.BeginHorizontal();

        GUIContent label = new GUIContent("Choose InputAction");
        index = EditorGUILayout.Popup(label, index, actionNames);

        if (GUILayout.Button($"Set HintButton"))
            references.SetHintButton(actions[index], inputAsset, false);

        GUILayout.EndHorizontal();
    }


    void Setup()
    {
        inputAsset = new InputAsset();
        foreach (InputAction action in inputAsset)
            actions.Add(action);
        actionNames = actions.Select(a => a.actionMap.name + '.' + a.name).ToArray();
    }





}
#endif
