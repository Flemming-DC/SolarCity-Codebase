using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rebinder : MonoBehaviour
{
    public static bool isRebinding { get; private set; }
    public static event Action<InputAction> onRebindStarted;
    public static event Action<InputAction> onRebindComplete;
    static InputActionRebindingExtensions.RebindingOperation rebindingOperation;



    public static void Rebind(InputAction action)
    {
        action.Disable();
        string[] forbiddenBindings = InputManager.always.ToggleItemUsage
                                    .bindings.Select(b => b.path).ToArray();

        rebindingOperation = action.PerformInteractiveRebinding()
                                   .WithControlsExcluding("Mouse")
                                   .WithControlsExcluding(forbiddenBindings[0])
                                   .WithControlsExcluding(forbiddenBindings[1]) //nb: here we assume precisely two paths
                                   .OnMatchWaitForAnother(0.1f)
                                   .WithBindingMask(InputBinding.MaskByGroup(InputManager.device))
                                   .OnCancel(operation => operation.Dispose())
                                   .OnComplete(operation => OnRebindComplete(action))
                                   .Start();
        action.Enable();
        onRebindStarted?.Invoke(action);
        isRebinding = true;
    }


    static void OnRebindComplete(InputAction action)
    {
        isRebinding = false;
        onRebindComplete?.Invoke(action);
        rebindingOperation.Dispose();
        action.Enable();

    }


    public static void OnApplicationQuit()
    {
        rebindingOperation?.Cancel();
    }


}

