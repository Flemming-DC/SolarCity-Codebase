using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ASettingsCanvas : MonoBehaviour
{
    public static Action<bool> onToggleCheapGraphics; // bool useCheapGraphics
    public static bool useCheapGraphics { get; protected set; }
}
