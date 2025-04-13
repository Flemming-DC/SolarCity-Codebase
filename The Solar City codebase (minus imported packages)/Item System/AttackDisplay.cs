using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class AttackDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text attackName;
    [SerializeField] TMP_Text otherData;

    public void Show(AttackDisplayData attackData)
    {
        gameObject.SetActive(true);

        attackName.text = attackData.name;

        string blockableStr = attackData.blockable ? "" : "can break shields\n";
        string toppleHitStr = attackData.canHitToppledFoes ? "can hit toppled foes\n" : "";
        string toppleStr = attackData.canToppleFoes ? "can topple foes\n" : "";

        otherData.text = $"" +
            $"damage:             {Round(attackData.damage)}\n" + // these variables appears directly above each other in unity, despite not doing so in the code
            $"attack speed:      {Round(attackData.attackSpeed, 1)}\n" +
            $"windup duration: {Round(attackData.windUpDuration, 1)}\n" +
            $"\n" +
            $"{blockableStr}" +
            $"{toppleHitStr}" +
            $"{toppleStr}" +
            //$"{attackData.description.Replace("\n", "")}\n" +
            //$"canHitMultipleTargets = {attackData.canHitMultipleTargets}\n" +
            $"";

    }

    float Round(float number, int decimalCount = 0)
    {
        int factor = (int)Mathf.Pow(10, decimalCount);
        return Mathf.Round(factor * number) / factor;
    }

}
