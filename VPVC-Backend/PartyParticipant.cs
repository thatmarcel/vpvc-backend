using VPVC_Backend.Shared;

namespace VPVC_Backend; 

public class PartyParticipant {
    public string id;
    public Guid socketId;
    public string userDisplayName;
    public bool isPartyLeader;
    
    public int teamIndex = -1;
    
    public int gameState = GameStates.lobby;
    public int relativePositionX;
    public int relativePositionY;

    public PartyParticipant(string id, Guid socketId, string userDisplayName, bool isPartyLeader) {
        this.id = id;
        this.socketId = socketId;
        this.userDisplayName = userDisplayName;
        this.isPartyLeader = isPartyLeader;
    }

    public SerializablePartyParticipant ToSerializable() {
        return new SerializablePartyParticipant {
            id = id,
            teamIndex = teamIndex,
            userDisplayName = userDisplayName,
            isPartyLeader = isPartyLeader
        };
    }
    
    public SerializablePartyParticipantState ToSerializableState() {
        return new SerializablePartyParticipantState {
            participantId = id,
            gameState = gameState,
            relativePositionX = relativePositionX,
            relativePositionY = relativePositionY
        };
    }
}