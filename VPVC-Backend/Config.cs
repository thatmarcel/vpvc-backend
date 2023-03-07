namespace VPVC_Backend; 

public static class Config {
    public static readonly int teamCount = 2;
    public static readonly int maxParticipantsPerTeam = 5;
    public static readonly int maxParticipantsPerParty = maxParticipantsPerTeam * teamCount;

    public static readonly TimeSpan participantStatesUpdateInterval = TimeSpan.FromSeconds(0.5);
}