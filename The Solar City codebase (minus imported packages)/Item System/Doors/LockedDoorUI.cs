using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LockedDoorUI : MonoBehaviour
{
    [SerializeField] TMP_Text message;
    [SerializeField] Image icon;


    static LockedDoorUI instance;

    void Awake()
    {
        if (instance != null)
            Debug.LogWarning($"instance is already set");
        instance = this;
    }

    public static void Show(string message_, Sprite icon_ = null)
    {
        instance.GetComponent<AWindow>().Open(setParent: false);
        instance.message.text = message_;
        instance.icon.sprite = icon_;
        instance.icon.gameObject.SetActive(icon_ != null);
    }


}
