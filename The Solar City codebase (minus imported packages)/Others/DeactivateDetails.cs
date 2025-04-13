using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateDetails : MonoBehaviour
{
    [SerializeField] Transform exceptionsContainer;
    [SerializeField] List<GameObject> detailContainers;
    [SerializeField] List<GameObject> exceptions;

    void Awake() => ASettingsCanvas.onToggleCheapGraphics += SetDeactivation;

    void OnDestroy() => ASettingsCanvas.onToggleCheapGraphics -= SetDeactivation;


    public void ToggleActivation()
    {
        bool active = detailContainers.Any(dc => dc.gameObject.activeSelf);
        SetDeactivation(active);
    }

    public void SetDeactivation(bool deactivate)
    {
        foreach (var container in detailContainers)
            container.SetActive(!deactivate);

        foreach (var exception in exceptions)
            exception.transform.parent = exceptionsContainer;
    }


}
