using System.Collections.Generic;
using UnityEngine;

public class DeactivateTheRemote : MonoBehaviour
{
    [SerializeField] float deactivationDistance;
    [SerializeField] float performanceTimerIntervalDistance;
    [SerializeField] float updateTimerInterval;
    [SerializeField] Transform player;

    float updateTimer;
    Vector3 playerPosition;
    //public static Dictionary<GameObject, List<UpdateTimer>> timersByCharacter = new();


    private void Update()
    {
        if (Time.timeSinceLevelLoad < 4 * Time.deltaTime)
            return;

        updateTimer -= Time.deltaTime;
        if (updateTimer <= 0)
        {
            updateTimer = updateTimerInterval;
            UpdatePerformance();
        }

    }

    void UpdatePerformance()
    {
        if (player != null)
            playerPosition = player.position;

        foreach (var character in CharacterSet.characters)
        {
            float sqrDistance = (character.transform.position - playerPosition).sqrMagnitude;
            bool activate = sqrDistance <= deactivationDistance * deactivationDistance;
            if (character.activeSelf != activate)
                character.SetActive(activate);

            //if (activate)
            //    HandlePerformanceTimers(character, sqrDistance);
        }
    }
    /*
    void HandlePerformanceTimers(GameObject character, float sqrDistance)
    {
        bool prioritizePerformance = sqrDistance >=
            performanceTimerIntervalDistance * performanceTimerIntervalDistance;

        foreach (var timer in timersByCharacter[character])
            timer.isNear = !prioritizePerformance;
    }
    */

    private void OnDrawGizmosSelected()
    {
        if (player == null)
            return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(player.position, deactivationDistance);
    }
}
