using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class BattleArea : MonoBehaviour
{
    [SerializeField] bool removeLeavers;
    [SerializeField] Transform enemiesToKillContainer;
    [SerializeField] List<APathBlocker> pathBlockers;

    List<ADamagable> enemiesToKill;
    AudioSource battleMusic;
    Bounds battleAreaBounds;

    void Start()
    {
        if (!GetComponent<Collider>().isTrigger)
            Debug.LogWarning($"The collider on {name} must be a trigger.");
        battleAreaBounds = GetComponent<Collider>().bounds;
        battleMusic = GetComponent<AudioSource>();

        enemiesToKill = enemiesToKillContainer
            .GetComponentsInChildren<ADamagable>()
            .Where(d => d.transform.parent.TryGetComponent(out NavMeshAgent _)) // this line excludes shield-damagables
            .ToList();
        enemiesToKill.ForEach(d => d.OnDie += () => OnLeaveOrDie(d));
    }

    void OnDestroy()
    {
        enemiesToKill.ForEach(d => d.OnDie -= () => OnLeaveOrDie(d));
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != PlayerReferencer.player)
            return;
        if (enemiesToKill.Count == 0)
            return;

        // start fight
        if (battleMusic != null && !battleMusic.isPlaying)
            battleMusic.Play();
        foreach (var blocker in pathBlockers)
            blocker.SetPathBlocked(true);
    }
    
    void OnTriggerExit(Collider other)
    {
        if (!removeLeavers)
            return;
        if (!other.TryGetComponent(out ADamagable damagable))
            return;
        if (!enemiesToKill.Contains(damagable))
            return;
        OnLeaveOrDie(damagable);
    }

    void OnLeaveOrDie(ADamagable deadEnemy)
    {
        enemiesToKill.Remove(deadEnemy);
        if (removeLeavers)
            enemiesToKill.RemoveAll(e => !battleAreaBounds.Contains(e.transform.position));

        if (enemiesToKill.Count > 0)
            return;

        // stop fight
        if (battleMusic != null && battleMusic.isPlaying)
            battleMusic.Stop();
        foreach (var blocker in pathBlockers)
            blocker.SetPathBlocked(false);
    }



}
