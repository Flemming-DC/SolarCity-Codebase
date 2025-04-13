using UnityEngine;
using TMPro;

public class Messenger : MonoBehaviour
{
    [SerializeField] float hintDuration = 4;
    [SerializeField] TMP_Text hintText;
    [SerializeField] TMP_Text poisonText;

    static Messenger instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogWarning($"instance is already set.\n {transform.Path()} is setting it again.");

        instance.hintText.enabled = false;
        instance.poisonText.enabled = false;
    }

    public static void ShowHint(string message)
    {
        instance.hintText.enabled = true;
        instance.hintText.text = message;
        instance.Delay(() => instance.hintText.enabled = false, instance.hintDuration);
    }

    public static void ShowPoison()
    {
        instance.poisonText.enabled = true;
        instance.Delay(() => instance.poisonText.enabled = false, instance.hintDuration);
    }





}
