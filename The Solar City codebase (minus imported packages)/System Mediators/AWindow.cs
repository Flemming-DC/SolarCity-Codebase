using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AWindow : MonoBehaviour
{

    public abstract void Open(bool setParent);

    public abstract void Close(bool openParentWindow);

    public abstract void SetDefaultButton(Button newDefault);

}
