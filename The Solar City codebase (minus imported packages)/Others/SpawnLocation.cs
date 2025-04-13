using UnityEngine;
using ConditionalField;

public class SpawnLocation : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] float durationBetweenSpawn = 60;
    [SerializeField] bool disableNearPlayer;
    //[ConditionalField(nameof(disableNearPlayer))] [SerializeField] GameObject player;
    [ConditionalField(nameof(disableNearPlayer))] [SerializeField] float nearPlayerDistance = 10;

    private void Update()
    {
        if (Random.value > Time.deltaTime / durationBetweenSpawn)
            return;
        if (PlayerReferencer.player == null)
            return;
        if (disableNearPlayer)
        {
            float sqrDistanceToPlayer = (transform.position - PlayerReferencer.player.transform.position).sqrMagnitude;
            if (sqrDistanceToPlayer < nearPlayerDistance * nearPlayerDistance)
                return;
        }

        GameObject instance = Instantiate(prefab);
        instance.transform.position = transform.position;
    }


    private void OnDrawGizmosSelected()
    {
        if (!disableNearPlayer)
            return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, nearPlayerDistance);
    }


}
