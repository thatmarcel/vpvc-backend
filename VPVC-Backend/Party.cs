﻿namespace VPVC_Backend; 

public class Party {
    public string id;
    public string joinCode;

    public readonly List<PartyParticipant> participants = new();

    public Party(string id, string joinCode) {
        this.id = id;
        this.joinCode = joinCode;
    }
}