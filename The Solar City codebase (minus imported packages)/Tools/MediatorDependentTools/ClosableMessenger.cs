using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClosableMessenger : MonoBehaviour
{
    [SerializeField] GameObject okButton;

    static ClosableMessenger instance;

    void Awake()
    {
        if (instance != null)
            Debug.LogWarning($"instance is already set.\n {transform.Path()} is setting it again.");
        instance = this;

    }

    public static void Show(GameObject childPanel)
    {
        instance.GetComponent<AWindow>().Open(setParent: false);
        foreach (Transform child in instance.transform)
            child.gameObject.SetActive(false);

        childPanel.SetActive(true);
        instance.okButton.SetActive(true);

    }



}
