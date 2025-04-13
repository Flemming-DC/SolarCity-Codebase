using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "ScriptableObject/Singleton/InputIconSet")]
public class InputIconSet : ScriptableObjectSingleton<InputIconSet>
{
    [SerializeField] GameObject hintPrefab;
    public Color Color_A = Color.green;
    public Color Color_X = Color.blue;
    public Color Color_B = Color.red;
    public Color Color_Y = Color.yellow;

    [SerializeField] GamepadIcons gamepadOutLine;
    [SerializeField] GamepadIcons gamepadFill;
    [SerializeField] Sprite keyBoardSquare;
    [SerializeField] Sprite keyBoardRectangle;
    [SerializeField] List<string> keyboardRectangleList;

    public static GamepadIcons GamepadOutLine { get => instance.gamepadOutLine; }
    public static GamepadIcons GamepadFill { get => instance.gamepadFill; }
    public static List<string> KeyboardRectangleList { get => instance.keyboardRectangleList; }

    public static GameObject ShowButton(InputAction inputAction, GameObject button = null, Vector2? position = null)
    {
        GameObject hintInstance = button == null ? Instantiate(instance.hintPrefab) : button;
        HintReferences references = hintInstance.GetComponentInChildren<HintReferences>();
        references.SetHintButton(inputAction, InputManager.inputAsset);

        if (position != null)
            foreach (Transform child in hintInstance.transform)
                child.GetComponent<RectTransform>().anchoredPosition = (Vector2) position;

        return hintInstance;
    }

    [Serializable]
    public struct GamepadIcons
    {
        [SerializeField] Sprite gamepadNoKey;
        [SerializeField] Sprite buttonSouth;
        [SerializeField] Sprite buttonNorth;
        [SerializeField] Sprite buttonEast;
        [SerializeField] Sprite buttonWest;
        [SerializeField] Sprite startButton;
        [SerializeField] Sprite selectButton;
        [SerializeField] Sprite leftTrigger;
        [SerializeField] Sprite rightTrigger;
        [SerializeField] Sprite leftShoulder;
        [SerializeField] Sprite rightShoulder;

        [SerializeField] Sprite dpadUp;
        [SerializeField] Sprite dpadDown;
        [SerializeField] Sprite dpadLeft;
        [SerializeField] Sprite dpadRight;

        [SerializeField] Sprite leftStickUp;
        [SerializeField] Sprite leftStickDown;
        [SerializeField] Sprite leftStickLeft;
        [SerializeField] Sprite leftStickRight;
        [SerializeField] Sprite rightStickUp;
        [SerializeField] Sprite rightStickDown;
        [SerializeField] Sprite rightStickLeft;
        [SerializeField] Sprite rightStickRight;

        [SerializeField] Sprite leftStickPress;
        [SerializeField] Sprite rightStickPress;
        [SerializeField] Sprite leftStick;
        [SerializeField] Sprite rightStick;


        public Sprite GetSprite(string bindingName)
        {
            // From the input system, we get the path of the control on device. So we can just
            // map from that to the sprites we have for gamepads.
            switch (bindingName)
            {
                case "A": return buttonSouth;
                case "Y": return buttonNorth;
                case "B": return buttonEast;
                case "X": return buttonWest;
                case "Start": return startButton;
                case "Select": return selectButton;
                case "LT": return leftTrigger;
                case "RT": return rightTrigger;
                case "LB": return leftShoulder;
                case "RB": return rightShoulder;
                
                case "D-Pad Up": return dpadUp;
                case "D-Pad Down": return dpadDown;
                case "D-Pad Left": return dpadLeft;
                case "D-Pad Right": return dpadRight;
                
                case "LS Up": return leftStickUp;
                case "LS Down": return leftStickDown;
                case "LS Left": return leftStickLeft;
                case "LS Right": return leftStickRight;
                case "RS Up": return rightStickUp;
                case "RS Down": return rightStickDown;
                case "RS Left": return rightStickLeft;
                case "RS Right": return rightStickRight;

                case "Left Stick Press": return leftStickPress;
                case "Right Stick Press": return rightStickPress;
                case "LS": return leftStick;
                case "RS": return rightStick;
                default: return gamepadNoKey;
            }
        }
    }





}

