using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    [SerializeField] Transform tabSelectionFrame;
    [SerializeField] Transform tabHoverFrame;
    [SerializeField] Transform tabButtonContainer;
    [SerializeField] Transform tabPageContainer;

    public static event Action<TabGroup, TabButton> OnTabChanged;
    TabButton firstTabButton;
    Dictionary<TabButton, GameObject> pageByButton = new Dictionary<TabButton, GameObject>();
    List<TabButton> tabButtons;
    TabButton currentTapButton;

    private void Awake()
    {
        tabButtons = tabButtonContainer.GetComponentsInDirectChildren<TabButton>();
        firstTabButton = tabButtons[0];
        currentTapButton = firstTabButton;
        List<Transform> pages = tabPageContainer.GetComponentsInDirectChildren<Transform>();
        for (int i = 0; i < tabButtons.Count; i++)
            pageByButton.Add(tabButtons[i], pages[i].gameObject);
        StartCoroutine(SetupTabsRoutine(pages));

        GetComponent<Window>().OnOpen += OnOpen;
    }

    private void Start()
    {
        InputManager.UI.GoToLeftTab.performed += _ => GoToNextTab(goRight: false);
        InputManager.UI.GoToRightTab.performed += _ => GoToNextTab(goRight: true);
    }

    private void OnDestroy() => GetComponent<Window>().OnOpen -= OnOpen;


    public void OnTabClickOrOpen(TabButton button)
    {
        if (currentTapButton == null)
            currentTapButton = firstTabButton;

        pageByButton[currentTapButton].SetActive(false);
        currentTapButton = button;
        pageByButton[button].SetActive(true);

        Button firstButtonInPage = pageByButton[currentTapButton].transform.GetChild(0).GetComponent<Button>();
        firstButtonInPage.Select();
        //this.Delay(() => firstButtonInPage.Select());
        tabSelectionFrame?.SetParent(button.transform, false);
        OnTabChanged?.Invoke(this, button);
    }

    public void OnTabEnter(TabButton button)
    {
        tabHoverFrame?.gameObject.SetActive(true);
        tabHoverFrame?.SetParent(button.transform, false);
    }


    public void OnTabExit()
    {
        tabHoverFrame?.gameObject.SetActive(false);
    }


    private void OnOpen()
    {
        bool unInitialized = (firstTabButton == null);
        if (unInitialized)
            return;
        OnTabClickOrOpen(currentTapButton);
    }


    void GoToNextTab(bool goRight)
    {
        if (!gameObject.activeSelf)
            return;

        int index = tabButtons.IndexOf(currentTapButton);
        index = goRight ? index + 1 : index - 1;
        if (index >= tabButtons.Count)
            index = 0;
        else if (index < 0)
            index = tabButtons.Count - 1;

        OnTabClickOrOpen(tabButtons[index]);
    }

    IEnumerator SetupTabsRoutine(List<Transform> pages)
    {
        pages.ForEach(p => p.gameObject.SetActive(true));
        yield return null;
        pages.ForEach(p => p.gameObject.SetActive(false));
        pageByButton[firstTabButton].SetActive(true);
    }

}
