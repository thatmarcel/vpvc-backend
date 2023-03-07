using ProtoBuf;

namespace VPVC_Backend.Shared.ProtobufMessages.ServerToClient; 

[ProtoContract]
public class PartyParticipantStatesUpdateMessageContent {
    [ProtoMember(1)]
    public List<SerializablePartyParticipantState>? partyParticipantStates { get; set; }
}