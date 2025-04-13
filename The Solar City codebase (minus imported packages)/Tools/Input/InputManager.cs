using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] bool allowDebugInBuild;

    public static InputAsset inputAsset;
    public static event Action<InputActionMap> onActionMapChanged;

    public static InputAsset.GameplayActions gameplay;
    public static InputAsset.UIActions UI;
    public static InputAsset.EditorActions editor;
    public static InputAsset.ItemUsageActions itemUsage;
    public static InputAsset.AlwaysActions always;
    public static InputAsset.DeathActions death;

    public static string device { get => playerInput.currentControlScheme; }
    public static bool isUsingKeyboard { get => device == inputAsset.KeyboardScheme.name; }
    public static bool isUsingGamepad { get => device == inputAsset.GamepadScheme.name; }
    public static PlayerInput playerInput;
    static InputManager instance;

    private void Awake()
    {
        instance = this;
        inputAsset = new InputAsset();
        playerInput = GetComponent<PlayerInput>();
        gameplay = inputAsset.Gameplay;
        UI = inputAsset.UI;
        editor = inputAsset.Editor;
        itemUsage = inputAsset.ItemUsage;
        always = inputAsset.Always;
        death = inputAsset.Death;

        always.Enable();
        if (Application.isEditor || allowDebugInBuild)
            editor.Enable();
        CheckControlSchemes();
    }

    private void OnEnable() => SetActionMap(gameplay);

    private void OnDisable() => inputAsset.Disable();



    public static void SetActionMap(InputActionMap actionMap, float delay = 0)
    {
        instance.StartCoroutine(SetActionMapRoutine(actionMap, delay));
    }

    static IEnumerator SetActionMapRoutine(InputActionMap actionMap, float delay = 0)
    {
        if (actionMap.enabled)
            yield break;
        yield return new WaitForSeconds(delay);
        if (!always.enabled)
            always.Enable();

        gameplay.Disable();
        UI.Disable();
        itemUsage.Disable();
        death.Disable();

        actionMap.Enable();
        onActionMapChanged?.Invoke(actionMap);
    }



    void CheckControlSchemes()
    {
        var controlSchemes = new InputControlScheme[] { inputAsset.KeyboardScheme, inputAsset.GamepadScheme };
        var missingSchemes1 = inputAsset.controlSchemes.Except(controlSchemes);
        var missingSchemes2 = controlSchemes.Except(inputAsset.controlSchemes);
        bool noMissingSchemes = missingSchemes1.Count() == 0 && missingSchemes2.Count() == 0;
        if (noMissingSchemes)
            return;
        Debug.LogError(
            $"inputAsset.controlSchemes == {inputAsset.controlSchemes}, " +
            $"but the code expects {controlSchemes}");

    }

}
