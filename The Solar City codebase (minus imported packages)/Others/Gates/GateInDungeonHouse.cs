using System.Collections;
using UnityEngine;

public class GateInDungeonHouse : Gate
{
    [SerializeField] BoxCollider visibilityZone;
    [SerializeField] BoxCollider closeGateZone;
    [SerializeField] GameObject lod0;
    [SerializeField] GameObject lod1;

    Transform player;

    void Start()
    {
        if (!visibilityZone.isTrigger)
            Debug.LogWarning($"visibilityZone.isTrigger should be true, but it is false.");
        if (!closeGateZone.isTrigger)
            Debug.LogWarning($"closeGateZone.isTrigger should be true, but it is false.");

        player = PlayerReferencer.player.transform;
    }



    private void Update()
    {
        if (player == null)
            return;

        bool isVisible = visibilityZone.bounds.Contains(player.position);
        lod0.SetActive(isVisible);
        lod1.SetActive(isVisible);

        if (!isVisible)
            return;
        bool close = closeGateZone.bounds.Contains(player.position);
        if (close && open)
            StartCoroutine(CloseRoutine());

    }


}
