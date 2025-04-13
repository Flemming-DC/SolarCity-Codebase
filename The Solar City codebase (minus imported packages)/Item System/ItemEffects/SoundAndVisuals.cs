using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundAndVisuals : ItemEffect
{
    [Header("------------ StatChange ------------")]

    [SerializeField] float visualsScale = 1;
    [SerializeField] float spawnPointHeight = 1;
    [SerializeField] Sound sound;
    [SerializeField] GameObject visualsPrefab;

    public override void Apply()
    {
        sound?.MakeSource(PlayerReferencer.player);
        sound?.Play(PlayerReferencer.player);

        if (visualsPrefab == null)
            return;
        GameObject visualsInstance = Object.Instantiate(
            visualsPrefab,
            PlayerReferencer.player.transform.position + spawnPointHeight * Vector3.up,
            Quaternion.identity);
        visualsInstance.transform.localScale *= visualsScale;

    }


}
