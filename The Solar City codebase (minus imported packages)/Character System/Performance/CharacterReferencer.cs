using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

[DefaultExecutionOrder(-10)]
public class CharacterReferencer : MonoBehaviour
{
    public GameObject Model { get; private set; }
    public GameObject Ragdoll { get; private set; }
    public GameObject Actions { get; private set; }
    public GameObject Plans { get; private set; }
    public GameObject GenericCharacterLogic { get; private set; }
    public GameObject genericAiLogic { get; private set; }

    public ITeamHandler teamHandler { get; private set; }
    public AnimancerComponent animancer { get; private set; }
    //public ActMachine actMachine { get; private set; }
    //public PlanMachine planMachine { get; private set; }

    private void Awake()
    {
        Model = this.GetComponentInDirectChildren<BodyParts>().gameObject;
        Ragdoll = this.GetComponentInDirectChildren<Ragdoll>().gameObject;
        Actions = this.GetComponentInDirectChildren<ActionState>().gameObject;
        GenericCharacterLogic = this.GetComponentInDirectChildren<ActMachine>().gameObject;
        teamHandler = this.GetComponentInDirectChildren<ITeamHandler>();
        animancer = Model.GetComponent<AnimancerComponent>();
        //actMachine = GenericCharacterLogic.GetComponent<ActMachine>();

        bool isPlayer = TryGetComponent(out PlayerReferencer _);
        if (!isPlayer)
        {
            Plans = this.GetComponentInDirectChildren<PlanState>().gameObject;
            genericAiLogic = this.GetComponentInDirectChildren<PlanMachine>().gameObject;
            //planMachine = genericAiLogic.GetComponent<PlanMachine>();
        }
    }




}
