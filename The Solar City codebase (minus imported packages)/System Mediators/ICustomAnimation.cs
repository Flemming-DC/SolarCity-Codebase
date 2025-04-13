using UnityEngine;
using Animancer;

public interface ICustomAnimation
{
    public bool isPlaying { get; }
    public bool TryPlay(ClipTransition animation, GameObject toolPrefab = null, Hand hand = Hand.right);


}
