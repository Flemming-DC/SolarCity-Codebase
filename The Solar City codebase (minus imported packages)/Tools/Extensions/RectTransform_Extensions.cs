using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectTransform_Extensions
{

    public static void SetRect(this RectTransform toBeSet, RectTransform toChangeInto)
    {
        toBeSet.anchorMin = toChangeInto.anchorMin;
        toBeSet.anchorMax = toChangeInto.anchorMax;
        toBeSet.anchoredPosition = toChangeInto.anchoredPosition;
        toBeSet.sizeDelta = toChangeInto.sizeDelta;
    }


}
