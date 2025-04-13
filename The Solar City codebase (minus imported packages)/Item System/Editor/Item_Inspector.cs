#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Item))]
public class Item_Inspector : Editor
{
    static int index;
    Type[] allEffectTypes;
    string[] allEffectNames;

    public override void OnInspectorGUI()
    {
        Item item = (Item)target;
        if (allEffectTypes == null || allEffectNames == null)
            Setup();

        Texture2D iconTexture = AssetPreview.GetAssetPreview(item.icon);
        GUILayout.Label(iconTexture, GUILayout.Width(50), GUILayout.Height(50));

        base.OnInspectorGUI();

        DisplayChooseItemEffect(item);
    }


    void DisplayChooseItemEffect(Item item)
    {
        GUILayout.BeginHorizontal();

        GUIContent label = new GUIContent("Choose ItemEffect");
        index = EditorGUILayout.Popup(label, index, allEffectNames);
        if (GUILayout.Button($"Add {allEffectNames[index]} effect"))
        {
            ItemEffect effect = (ItemEffect)Activator.CreateInstance(allEffectTypes[index]);
            effect.item = item;
            item.itemEffects.Add(effect);
        }

        GUILayout.EndHorizontal();
    }


    void Setup()
    {
        allEffectTypes = GetSubClasses(typeof(ItemEffect));
        allEffectNames = allEffectTypes.Select(e => e.Name).ToArray();
    }

    Type[] GetSubClasses(Type parentClass)
    {
        return parentClass.Assembly.GetTypes().Where(type => type.IsSubclassOf(parentClass)).ToArray();
    }

}
#endif
