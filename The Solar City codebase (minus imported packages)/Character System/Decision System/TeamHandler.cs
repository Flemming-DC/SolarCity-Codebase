using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamHandler : MonoBehaviour, ITeamHandler
{
    [SerializeField] Team team;
    [SerializeField] List<Team> foes;
    [SerializeField] float minDamageTakenBeforeGoingHostile = 10;
    [SerializeField] bool willAttackTeamLess;
    //[SerializeField] List<Team> friends;
    //[SerializeField] bool willDefendTeamLess;

    public Team GetTeam() => team;
    CharacterDamagable damagable;
    Dictionary<GameObject, float> damageTakenDict = new Dictionary<GameObject, float>();

    private void OnEnable()
    {
        damagable = this.GetComponentInSiblings<CharacterDamagable>();
        damagable.OnHealthChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        damagable.OnHealthChanged -= OnHealthChanged;
    }

    public bool IsHostileTo(GameObject target)
    {
        if (target == null)
            return false;
        if (damageTakenDict.ContainsKey(target))
            if (damageTakenDict[target] > minDamageTakenBeforeGoingHostile)
                return true;

        var targetTeam = target?.GetComponent<CharacterReferencer>()?.teamHandler;
        if (targetTeam == null)
            return willAttackTeamLess;
        else
            return foes.Contains(targetTeam.GetTeam());
    }
    /*
    public bool IsFriendlyTo(GameObject target)
    {
        TeamHandler targetTeam = target.GetComponentInCharacter<TeamHandler>();

        if (targetTeam == null)
            return willDefendTeamLess;
        else
            return friends.Contains(targetTeam.team);
    }
    */

    void OnHealthChanged(GameObject attacker, float oldHealth, float health)
    {
        if (attacker == null)
            return;
        if (!attacker.TryGetComponentInCharacter(out CharacterDamagable _))
            return;

        if (!damageTakenDict.ContainsKey(attacker))
            damageTakenDict.Add(attacker, 0);

        damageTakenDict[attacker] += oldHealth - health;
    }


}
