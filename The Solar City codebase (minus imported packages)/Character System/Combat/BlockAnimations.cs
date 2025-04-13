using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

[CreateAssetMenu(menuName = "ScriptableObject/BlockAnimations")]
public class BlockAnimations : ScriptableObject
{
    //public float blockStartNormalizedEndTime = 1;
    public string blockStartClipName;
    public string blockLoopClipName;
    public string blockHitClipName;

    public ClipTransitionAsset.UnShared blockStart;
    public ClipTransitionAsset.UnShared blockLoop;
    public ClipTransitionAsset.UnShared blockHit;

}
