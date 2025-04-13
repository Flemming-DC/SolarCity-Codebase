using UnityEngine;
using UnityEditor;


#if UNITY_EDITOR
[CustomEditor(typeof(Attack))]
public class Attack_Inspector : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Attack attack = (Attack)target;

        if (attack.blockable && attack.unRecoilable)
        {
            string message = $"This attack is blockable, but not recoilable, which is likely to look bad when blocked";
            EditorGUILayout.HelpBox(message, MessageType.Warning, true);
        }
        if (attack.animation.Clip != null)
            GUILayout.Label($"windUpDuration = {attack.GetWindUpDuration()}");
        if (attack.animation.Clip != null)
            GUILayout.Label($"DPS (in combo and one hit per attack) = {attack.damage * attack.GetAttackSpeed(true)}");
        if (attack.animation.Clip != null)
            GUILayout.Label($"DPS (out of combo and one hit per attack)= {attack.damage * attack.GetAttackSpeed(false)}");

    }


}
#endif