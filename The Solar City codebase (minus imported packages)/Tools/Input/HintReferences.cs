using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class HintReferences : MonoBehaviour
{
    [SerializeField] Image fill;
    [SerializeField] Image outLine;
    [SerializeField] Image squareBackground;
    [SerializeField] TMP_Text squareText;
    [SerializeField] Image rectangleBackground;
    [SerializeField] TMP_Text rectangleText;

    public bool useRectangular { private get; set; }
    public static string tapPrefix = "Tap ";
    public static string holdPrefix = "Hold ";

    private void OnEnable()
    {
        Invoke(nameof(SetActive), Time.deltaTime);
        if (Time.timeSinceLevelLoad > 2 * Time.deltaTime)
            InputManager.playerInput.onControlsChanged += _ => SetActive();

    }
    private void OnDisable()
    {
        InputManager.playerInput.onControlsChanged -= _ => SetActive();
    }



    public void SetHintButton(InputAction inputAction, InputAsset inputAsset, bool activate = true)
    {
        if (inputAsset == null)
            inputAsset = new InputAsset();
        if (activate)
            SetActive(); // I don't know whether this initial activation is necessary

        //Gamepad
        string gamepadBinding = inputAction
            .GetBindingDisplayString(InputBinding.MaskByGroup(inputAsset.GamepadScheme.name))
            .TrimStart(tapPrefix)
            .TrimStart(holdPrefix);
        Color color;
        switch (gamepadBinding)
        {
            case "A": color = InputIconSet.instance.Color_A; break;
            case "Y": color = InputIconSet.instance.Color_Y; break;
            case "B": color = InputIconSet.instance.Color_B; break;
            case "X": color = InputIconSet.instance.Color_X; break;
            default: color = Color.white; break;
        }
        fill.sprite = InputIconSet.GamepadFill.GetSprite(gamepadBinding);
        outLine.sprite = InputIconSet.GamepadOutLine.GetSprite(gamepadBinding);
        outLine.color = color;

        // Keyboard
        string keyboardBinding = inputAction
            .GetBindingDisplayString(InputBinding.MaskByGroup(inputAsset.KeyboardScheme.name))
            .TrimStart(tapPrefix);
        keyboardBinding = CorrectUnReadableKeys(keyboardBinding);


        useRectangular = InputIconSet.KeyboardRectangleList.Contains(keyboardBinding, StringComparer.OrdinalIgnoreCase);
        squareText.text = keyboardBinding;
        rectangleText.text = keyboardBinding;

        if (gameObject.name.Contains("Right Attack 1"))
        {
            InputIconSet.KeyboardRectangleList.Print();
            print(keyboardBinding);
            print(useRectangular);
        }

        if (activate)
            SetActive(); // we activate again to handle the changes to useRectangular
    }


    void SetActive()
    {
        useRectangular = InputIconSet.KeyboardRectangleList.Contains(rectangleText.text, StringComparer.OrdinalIgnoreCase);

        foreach (Transform child in transform)
            child.gameObject.SetActive(false);

        if (InputManager.isUsingGamepad)
        {
            outLine.gameObject.SetActive(true);
            fill.gameObject.SetActive(true);
        }
        else if (InputManager.isUsingKeyboard)
        {
            if (useRectangular)
            {
                rectangleBackground.gameObject.SetActive(true);
                rectangleText.gameObject.SetActive(true);
            }
            else
            {
                squareBackground.gameObject.SetActive(true);
                squareText.gameObject.SetActive(true);
            }
        }
        else
            Debug.LogWarning($"device {InputManager.device} is not recognized");

    }

    string CorrectUnReadableKeys(string keyboardBinding)
    {
        if (keyboardBinding == "Delta")
            return "Mouse";
        if (keyboardBinding == "LMB")
            return "Mouse Left";
        if (keyboardBinding == "RMB")
            return "Mouse Right";
        if (keyboardBinding == "MMB")
            return "Mouse Middle";
        if (keyboardBinding == "Mellemrum")
            return "Space";
        if (keyboardBinding == "Tilbage")
            return "Backspace";
        
        return keyboardBinding;
    }

}
