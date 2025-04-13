using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Window : AWindow
{
    [SerializeField] Button defaultButton;
    [SerializeField] bool deactivateParent;

    public static Button currentDefaultButton { get => currentWindow?.defaultButton; }
    public event Action OnOpen;
    public event Action OnClose;
    static Window currentWindow;
    Window parentWindow;
    Button selectionDuringOpening;

    private void Start()
    {
        InputManager.UI.Close.performed += _ => OnCloseHotkeyPressed();
    }



    public override void Open(bool setParent)
    {
        PreventFakeButtonClicks();
        InputManager.SetActionMap(InputManager.UI);

        if (setParent)
            parentWindow = currentWindow;
        currentWindow = this;

        gameObject.SetActive(true);
        //if (setParent && deactivateParent)
        //    parentWindow?.gameObject.SetActive(false);
        if (setParent && deactivateParent)
            parentWindow?.Close(false);
        defaultButton?.Select();
        OnOpen?.Invoke();
    }


    public override void Close(bool openParent)
    {
        gameObject.SetActive(false);
        OnClose?.Invoke();
        if (openParent)
            parentWindow?.Open(false);

        int openWindowCount = MenuActivator
            .allWindows.Where(w => w.gameObject.activeSelf).ToArray().Length;
        if (openWindowCount == 0)
        {
            InputManager.SetActionMap(InputManager.gameplay);
            currentWindow = null;
            parentWindow = null;
        }

    }

    void OnCloseHotkeyPressed()
    {
        if (currentWindow == this)
            //Close(true);
            this.Delay(() => Close(true)); // why delay?
    }



    void PreventFakeButtonClicks()
    {
        selectionDuringOpening = EventSystem.current.currentSelectedGameObject?.GetComponent<Button>();

        if (selectionDuringOpening != null)
            selectionDuringOpening.interactable = false;
        if (defaultButton != null)
            defaultButton.interactable = false;

        Invoke(nameof(AllowGenuineButtonClicks), 2 * Time.deltaTime);
    }

    void AllowGenuineButtonClicks()
    {
        if (selectionDuringOpening != null)
            selectionDuringOpening.interactable = true;
        if (defaultButton != null)
            defaultButton.interactable = true;
    }

    public override void SetDefaultButton(Button newDefault)
    {
        if (newDefault == null)
            Debug.LogWarning($"cannot set default button to null");
        else
            defaultButton = newDefault;
    }


}
