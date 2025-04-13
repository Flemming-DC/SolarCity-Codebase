using System;
using System.Collections.Generic;
using UnityEngine;

public class HandSelector : MonoBehaviour
{
    static AWindow handSelectionWindow;
    static Action<Hand> function;
    public static bool isChoosingHand;

    private void Awake()
    {
        handSelectionWindow = GetComponent<AWindow>();
    }


    public static void CallWithHand(Action<Hand> function_)
    {
        function = function_;
        handSelectionWindow.Open(true);
        isChoosingHand = true;
    }

    public void OnRightHandChosen()
    {
        function(Hand.right);
        handSelectionWindow.Close(true);
        isChoosingHand = false;
    }

    public void OnLeftHandChosen()
    {
        function(Hand.left);
        handSelectionWindow.Close(true);
        isChoosingHand = false;
    }


}
