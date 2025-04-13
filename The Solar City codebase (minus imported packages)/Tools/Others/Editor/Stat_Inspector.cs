using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Stat))]
public class Stat_Inspector : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, label);

        SerializedProperty constantValue = property.FindPropertyRelative("baseValue");
        EditorGUI.PropertyField(position, constantValue, GUIContent.none);
        EditorGUI.EndProperty();
    }
}
#endif