using UnityEngine;

public abstract class ALockOnRotator : MonoBehaviour
{
    public abstract bool lockOn { get; protected set; }
    public abstract Transform target { get; protected set; }
}
