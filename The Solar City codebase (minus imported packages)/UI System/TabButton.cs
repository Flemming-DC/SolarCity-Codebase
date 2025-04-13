using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TabButton : Button, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    TabGroup tabGroup;

    override protected void Awake()
    {
        base.Awake();
        onClick.AddListener(OnClick);
        tabGroup = GetComponentInParent<TabGroup>();
    }

    void OnClick()
    {
        tabGroup.OnTabClickOrOpen(this);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit();
    }
    

}
