using UnityEngine;
using UnityEngine.InputSystem;

public class RebindInfo : MonoBehaviour
{
    
    private void Start()
    {
        Rebinder.onRebindStarted += ShowInfo;
        Rebinder.onRebindComplete += HideInfo;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Rebinder.onRebindStarted -= ShowInfo;
        Rebinder.onRebindComplete -= HideInfo;
    }
    
    void ShowInfo(InputAction _)
    {
        gameObject?.SetActive(true);
    }

    void HideInfo(InputAction _)
    {
        gameObject?.SetActive(false);
    }
}
