
public interface ITeamHandler
{
    public Team GetTeam();
}

public enum Team { Player, FriendlyNpcs, Enemies }