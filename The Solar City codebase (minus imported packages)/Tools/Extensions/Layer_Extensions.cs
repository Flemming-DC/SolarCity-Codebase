using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Layer_Extensions
{
    public static bool IsInLayerMask(this int layer, LayerMask layerMask)
    {
        return layerMask == (layerMask | (1 << layer));
    }

    public static bool IsInLayerMask(this Component component, LayerMask layerMask)
    {
        int layer = component.gameObject.layer;
        return layer.IsInLayerMask(layerMask);
    }


    public static LayerMask GetLayerMask(this int layer)
    {
        return (1 << layer);
    }

    public static int GetFirstLayer(this LayerMask mask)
    {
        for (int layer = 0; layer < 32; layer++)
            if (layer.IsInLayerMask(mask))
                return layer;

        Debug.LogWarning($"Failed to find layer in LayerMask {mask}. Returning -1 by default.");
        return -1;
    }


}
