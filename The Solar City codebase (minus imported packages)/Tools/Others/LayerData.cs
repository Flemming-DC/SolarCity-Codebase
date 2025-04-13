using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Singleton/LayerData")]
public class LayerData : ScriptableObjectSingleton<LayerData>
{
    public LayerMask characterMask;
    public LayerMask hitableMask;
    public LayerMask terrainMask;
    public LayerMask meshBreakerMask;

    private void OnEnable()
    {
        CheckIfMaskIsSingleton(characterMask);
        CheckIfMaskIsSingleton(terrainMask);
        CheckIfMaskIsSingleton(meshBreakerMask);
    }

    void CheckIfMaskIsSingleton(LayerMask mask)
    {
        int counts = 0;
        for (int layer = 0; layer < 32; layer++)
            if (layer.IsInLayerMask(mask))
                counts++;
        if (counts != 1)
            Debug.LogWarning($"the layerMask {mask} should have presicely one layer in it.");
    }


}
