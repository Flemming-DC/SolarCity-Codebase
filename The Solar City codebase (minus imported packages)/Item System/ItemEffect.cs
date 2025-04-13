using System;
using UnityEngine;

[Serializable]
public abstract class ItemEffect : StartReceiver
{
    public Item item { get; set; }

    public ItemEffect()
    {
        BehaviourEventCaller.startReceivers.Add(this);
    }


    public virtual void Start() { }
    public abstract void Apply();
    public virtual void Cease() { }


}
