using VPVC_Backend.Shared;
using VPVC_Backend.Shared.ProtobufMessages;
using VPVC_Backend.Shared.ProtobufMessages.ServerToClient;

namespace VPVC_Backend; 

public static class Parties {
    public static List<Party> all = new();
    public static Dictionary<Guid, Party> forSocketIds = new();

    private static Timer? participantStatesUpdateTimer;

    public static Party? PartyWithId(string partyId) {
        return all.FirstOrDefault(party => party.id == partyId);
    }

    public static Party? PartyForSocketId(Guid socketId) {
        if (forSocketIds.ContainsKey(socketId)) {
            return forSocketIds[socketId];
        } else {
            return null;
        }
    }
    
    public static Party CreateParty() {
        var partyId = RandomString.Generate();
        var partyJoinCode = RandomString.GenerateUsingOnlyUppercaseLetters(6);

        // If a party with the generated id or join code already exists, try again.
        if (all.FirstOrDefault(party => party.id == partyId || party.joinCode == partyJoinCode) != null) {
            return CreateParty();
        }

        var party = new Party(partyId, partyJoinCode);
        
        all.Add(party);

        return party;
    }

    public static void DeleteParty(Party party) {
        // TODO: Notify all participants about party deletion

        foreach (var partyParticipant in party.participants) {
            forSocketIds.Remove(partyParticipant.socketId);
        }
        
        all.Remove(party);
    }

    public static void DeleteParty(string partyId) {
        var party = PartyWithId(partyId);

        if (party == null) {
            return;
        }
        
        DeleteParty(party);
    }
    
    // Returns the joined party, if successful
    public static Party? JoinParty(Party party, Guid socketId, string userDisplayName, bool isPartyLeader = false) {
        if (party.participants.Count >= Config.maxParticipantsPerParty) {
            return null;
        }
        
        var participantId = RandomString.Generate(4);

        // If a participant with the generated id already exists, try again.
        if (party.participants.FirstOrDefault(participant => participant.id == participantId) != null) {
            return JoinParty(party, socketId, userDisplayName);
        }

        var partyParticipant = new PartyParticipant(participantId, socketId, userDisplayName, isPartyLeader);

        for (int teamIndex = 0; teamIndex < Config.teamCount; teamIndex++) {
            var isTeamFull = party.participants.Count(participant => participant.teamIndex == teamIndex) >= Config.maxParticipantsPerTeam;
            
            if (!isTeamFull) {
                partyParticipant.teamIndex = teamIndex;
                break;
            }
        }

        if (partyParticipant.teamIndex == -1) {
            return null;
        }
        
        party.participants.Add(partyParticipant);

        forSocketIds[socketId] = party;
        
        InformParticipantsAboutPartyParticipantsChange(party, participantId);

        return party;
    }
    
    // Returns the joined party, if successful
    public static Party? JoinParty(string partyJoinCode, Guid socketId, string userDisplayName) {
        var party = all.FirstOrDefault(party => party.joinCode == partyJoinCode);

        if (party == null) {
            Logger.Log("Party not found.");
            return null;
        }

        return JoinParty(party, socketId, userDisplayName);
    }

    // Returns the new party, if successful
    public static Party? CreateAndJoinParty(Guid socketId, string userDisplayName) {
        var party = CreateParty();
        var wasPartyJoinSuccessful = JoinParty(party, socketId, userDisplayName, true) != null;

        if (!wasPartyJoinSuccessful) {
            DeleteParty(party);
            return null;
        }

        return party;
    }

    public static void LeaveParty(Guid socketId) {
        var party = PartyForSocketId(socketId);

        if (party == null) {
            return;
        }

        var partyParticipant = party.participants.FirstOrDefault(participant => participant.socketId == socketId);

        if (partyParticipant == null) {
            return;
        }

        party.participants.Remove(partyParticipant);

        if (partyParticipant.isPartyLeader && party.participants.Count > 0) {
            party.participants[0].isPartyLeader = true;
        }
        
        forSocketIds.Remove(socketId);

        if (party.participants.Count < 1) {
            DeleteParty(party);
            return;
        }

        InformParticipantsAboutPartyParticipantsChange(party);
    }

    // Returns whether successful and the new team index
    public static Tuple<bool, int> ChangeTeam(Guid socketId, int newTeamIndex) {
        var party = PartyForSocketId(socketId);

        if (party == null) {
            return new Tuple<bool, int>(false, -1);
        }

        var partyParticipant = party.participants.FirstOrDefault(participant => participant.socketId == socketId);

        if (partyParticipant == null) {
            return new Tuple<bool, int>(false, -1);
        }

        if (party.participants.Count(participant => participant.teamIndex == newTeamIndex) >= Config.maxParticipantsPerTeam) {
            return new Tuple<bool, int>(false, partyParticipant.teamIndex);
        }

        partyParticipant.teamIndex = newTeamIndex;
        
        InformParticipantsAboutPartyParticipantsChange(party, partyParticipant.id);

        return new Tuple<bool, int>(true, newTeamIndex);
    }

    private static void InformParticipantsAboutPartyParticipantsChange(Party party, string? excludedParticipantId = null) {
        foreach (var partyParticipant in party.participants) {
            if (excludedParticipantId == partyParticipant.id) {
                continue;
            }

            var resultMessage = new SessionMessage {
                type = MessageTypes.partyParticipantsChange,
                partyParticipantsChangeMessageContent = new PartyParticipantsChangeMessageContent {
                    partyParticipantSelf = partyParticipant.ToSerializable(),
                    partyParticipants = party.participants
                        .Where(participant => participant.id != partyParticipant.id)
                        .Select(participant => participant.ToSerializable())
                        .ToList()
                }
            };
            
            MessageSender.SendMessage(resultMessage, partyParticipant.socketId);
        }
    }

    public static void SetupParticipantStatesUpdateTimer() {
        participantStatesUpdateTimer = new Timer(_ => {
            try {
                SendParticipantStatesUpdates();
            } catch (Exception exception) {
                Logger.Log(exception.ToString());
            }
        }, null, Config.participantStatesUpdateInterval, Config.participantStatesUpdateInterval);
    }

    private static void SendParticipantStatesUpdates() {
        // Avoid looping through lists being modified
        
        foreach (var party in new List<Party>(all)) {
            if (party.participants.Count < 2) {
                continue;
            }

            foreach (var partyParticipant in new List<PartyParticipant>(party.participants)) {
                var resultMessage = new SessionMessage {
                    type = MessageTypes.partyParticipantStatesUpdate,
                    partyParticipantStatesUpdateMessageContent = new PartyParticipantStatesUpdateMessageContent {
                        partyParticipantStates = party.participants
                            .Where(participant => participant.id != partyParticipant.id)
                            .Select(participant => participant.ToSerializableState())
                            .ToList()
                    }
                };
            
                MessageSender.SendMessage(resultMessage, partyParticipant.socketId);
            }
        }
    }
}