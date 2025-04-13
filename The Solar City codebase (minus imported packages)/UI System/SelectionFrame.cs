using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SelectionFrame : MonoBehaviour
{
    [SerializeField] Transform frameTransform;
    [SerializeField] Selectable unPauseButton;
    [SerializeField] TMP_Text debugText;
    [SerializeField] bool debug;

    GameObject selection;
    GameObject lastSelection;
    RectTransform frameRect;

    void Awake()
    {
        frameRect = frameTransform.GetComponent<RectTransform>();
    }

    void Start()
    {
        InputManager.UI.Select.performed += _ => OnSelect();
        MenuActivator.onTogglePause += OnTogglePaused;
    }
    
    void OnDestroy()
    {
        InputManager.UI.Select.performed -= _ => OnSelect();
        MenuActivator.onTogglePause -= OnTogglePaused;
    }

    void Update()
    {
        HandleDebugPrint();
        if (UIisClosed())
            return;
        UpdateSelection();
        MakeSureThatSomethingIsSelected();
        UpdateSelectionFrame();
    }

    void HandleDebugPrint()
    {
        if (InputManager.editor.PrintSelectionFrame.WasPressedThisFrame())
            print($"The selectionFrame is located at:    {frameTransform.Path()}. \n" +
                  $"The selection is {selection.transform.Path()}");
    }

    void UpdateSelection()
    {
        if (selection != null)
            lastSelection = selection;
        selection = EventSystem.current.currentSelectedGameObject;
    }

    void UpdateSelectionFrame()
    {
        if (frameTransform.parent != selection.transform)
        {
            frameTransform.SetParent(selection.transform.parent, false);
            frameRect.SetRect(selection.GetComponent<RectTransform>());
            frameTransform.SetParent(selection.transform, true);
            debugText.text = "Selected UI: " + selection.name;
        }
        debugText.enabled = debug;
    }

    void MakeSureThatSomethingIsSelected()
    {
        // checking for missing selection
        if (Time.time < 3 * Time.deltaTime)
            return;
        if (SelectionIsValid(selection))
            return;
        //if (SelectionIsValid(lastSelection))
        //    return;

        // first attempted fix
        /*
        Debug.Log(
            $"No selection found. Setting selection to lastSelection." +
            $"This could be due to clicking outside the UI, while in UI mode." +
            $"\nselection = {selection}, lastSelection = {lastSelection}");
        */
        if (lastSelection != null && lastSelection.TryGetComponent(out Selectable lastSelectable))
        {
            selection = lastSelection;
            lastSelectable.Select();
        }
        if (SelectionIsValid(selection))
            return;

        // second attempted fix
        Debug.LogWarning(
            $"Still no selection found. Setting selection to currentDefaultButton." +
            $"\nselection = {selection}, currentDefaultButton = {Window.currentDefaultButton}");
        if (Window.currentDefaultButton != null)
        {
            selection = Window.currentDefaultButton.gameObject;
            Window.currentDefaultButton.Select();
        }
        if (SelectionIsValid(selection))
            return;

        // no further attempts
        Debug.LogWarning(
            $"Still no selection found. There are no known fixes." +
            $"\nselection = {selection}, scene = {SceneManager.GetActiveScene().name}");
    }

    bool SelectionIsValid(GameObject selection_)
    {
        if (UIisClosed())
            Debug.LogWarning("you are checking whether you have a selection, even though the UI is closed.");
        if (selection_ == null)
            return false;
        if (!selection_.activeInHierarchy) 
            return false;
        if (!selection_.TryGetComponent(out Selectable _))
            return false;
        
        return true;
    }
    
    bool UIisClosed()
    {
        if (!InputManager.UI.enabled)
            return true;
        if (MenuActivator.allWindows == null)
            return false;
        foreach (var window in MenuActivator.allWindows)
            if (window.gameObject.activeInHierarchy)
                return false;
        return true;
    }
    
    void OnSelect()
    {
        if (selection.TryGetComponent(out Button button))
            button.onClick?.Invoke();
    }

    void OnTogglePaused(bool paused)
    {
        selection = unPauseButton.gameObject;
        lastSelection = unPauseButton.gameObject;
        unPauseButton.Select();
        UpdateSelectionFrame();
    }

}
