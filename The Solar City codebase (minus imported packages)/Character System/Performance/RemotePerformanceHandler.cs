using System;
using System.Collections;
using UnityEngine;
using MenteBacata.ScivoloCharacterController;

public class RemotePerformanceHandler : MonoBehaviour
{
    [SerializeField] float radiusMultiplierForCheapGraphics = 0.5f;
    [SerializeField] CollisionEvent triggerNear;
    [SerializeField] CollisionEvent triggerMid;
    [SerializeField] CollisionEvent triggerFar;

    // bool = character has become near player
    public static event Action<Collider, bool> onCharacterNearPlayerChanged; 
    float nearDistanceSqr;
    float middleDistanceSqr;
    float farDistanceSqr;
    float safetyMargin = 1.3f;
    float radiusMultiplier = 1;
    float initialNearDistance;
    float initialMiddleDistance;
    float initialFarDistance;

    IEnumerator Start()
    {
        initialNearDistance = triggerNear.GetComponent<SphereCollider>().radius;
        initialMiddleDistance = triggerMid.GetComponent<SphereCollider>().radius;
        initialFarDistance = triggerFar.GetComponent<SphereCollider>().radius;
        ASettingsCanvas.onToggleCheapGraphics += onToggleCheapGraphics;

        for (int i=0; i<5; i++)
            yield return null;

        triggerNear.onTriggerEnter.AddListener((c) => onCharacterNearPlayerChanged?.Invoke(c, true));
        triggerNear.onTriggerExit.AddListener((c) => onCharacterNearPlayerChanged?.Invoke(c, false));

        triggerMid.onTriggerEnter.AddListener((c) => SetPerformance(c, safetyMargin));
        triggerFar.onTriggerEnter.AddListener((c) => SetPerformance(c, safetyMargin));
        triggerMid.onTriggerExit.AddListener((c) => SetPerformance(c, 1f / safetyMargin));
        triggerFar.onTriggerExit.AddListener((c) => SetPerformance(c, 1f / safetyMargin));

        foreach (var character in CharacterSet.characters)
            if (!character.TryGetComponent(out PlayerReferencer _))
            {
                var collider = character.GetComponent<Collider>(true);
                SetPerformance(collider, 1);
                float distanceSqr = (character.transform.position - transform.position).sqrMagnitude;
                onCharacterNearPlayerChanged?.Invoke(collider, distanceSqr <= nearDistanceSqr);
            }
    }

    private void OnDestroy()
    {
        ASettingsCanvas.onToggleCheapGraphics -= onToggleCheapGraphics;
    }


    void SetPerformance(Collider enemyCollider, float safetyMultiplier)
    {
        var referencer = enemyCollider.GetComponent<CharacterReferencer>(true);
        if (referencer == null)
            return;
        float distanceSqr = (referencer.transform.position - transform.position).sqrMagnitude;

        if (distanceSqr < safetyMultiplier * middleDistanceSqr)
            SetActivation(referencer, true, true);
        else if (distanceSqr < safetyMultiplier * farDistanceSqr)
            SetActivation(referencer, false, true);
        else
            SetActivation(referencer, false, false);

    }

    void SetActivation(CharacterReferencer referencer, bool activeCode, bool activeGraphics)
    {
        if (activeCode && !activeGraphics)
            Debug.LogWarning($"activeCode is true, but activeGraphics is false. This seems wrong.");

        referencer.Model.SetActive(activeGraphics);
        referencer.Ragdoll.SetActive(activeGraphics);

        if (!activeCode)
        {
            var actMachine = referencer.GenericCharacterLogic.GetComponent<ActMachine>(true);
            var planMachine = referencer.genericAiLogic.GetComponent<PlanMachine>(true);
            if (actMachine.InState<Falling>())
                return;

            actMachine.SetState<Idle>();
            planMachine.SetNextState(planMachine.initialState);
        }
        if (activeGraphics && !activeCode)
        {
            var idleAnimation = referencer
                .GenericCharacterLogic.GetComponent<AnimationUpdater>().animations.idle;
            referencer.animancer.Play(idleAnimation);
        }

        referencer.Actions.SetActive(activeCode);
        referencer.Plans.SetActive(activeCode);
        referencer.GenericCharacterLogic.SetActive(activeCode);
        referencer.genericAiLogic.SetActive(activeCode);
        referencer.GetComponent<CharacterCapsule>().enabled = activeCode;
    }



    void onToggleCheapGraphics(bool useCheapGraphics)
    {
        radiusMultiplier = useCheapGraphics ? radiusMultiplierForCheapGraphics : 1;

        triggerNear.GetComponent<SphereCollider>().radius = radiusMultiplier * initialNearDistance;
        triggerMid.GetComponent<SphereCollider>().radius = radiusMultiplier * initialMiddleDistance;
        triggerFar.GetComponent<SphereCollider>().radius = radiusMultiplier * initialFarDistance;

        nearDistanceSqr = Mathf.Pow(radiusMultiplier * initialNearDistance, 2);
        middleDistanceSqr = Mathf.Pow(radiusMultiplier * initialMiddleDistance, 2);
        farDistanceSqr = Mathf.Pow(radiusMultiplier * initialFarDistance, 2);
    }


}
