using ProtoBuf;
using VPVC_Backend.Shared.ProtobufMessages.ClientToServer;
using VPVC_Backend.Shared.ProtobufMessages.ServerToClient;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnassignedField.Global

namespace VPVC_Backend.Shared.ProtobufMessages; 

[ProtoContract]
public class SessionMessage {
    [ProtoMember(2)]
    public int type { get; set; }

    [ProtoMember(3)]
    public PartyCreateMessageContent? partyCreateMessageContent { get; set; }
    
    [ProtoMember(4)]
    public PartyCreateResultMessageContent? partyCreateResultMessageContent { get; set; }
    
    [ProtoMember(5)]
    public PartyJoinMessageContent? partyJoinMessageContent { get; set; }
    
    [ProtoMember(6)]
    public PartyJoinResultMessageContent? partyJoinResultMessageContent { get; set; }
    
    [ProtoMember(7)]
    public SelfStateUpdateMessageContent? selfStateUpdateMessageContent { get; set; }
    
    [ProtoMember(8)]
    public PartyParticipantsChangeMessageContent? partyParticipantsChangeMessageContent { get; set; }
    
    [ProtoMember(9)]
    public PartyParticipantStatesUpdateMessageContent? partyParticipantStatesUpdateMessageContent { get; set; }
    
    [ProtoMember(10)]
    public ChangeTeamMessageContent? changeTeamMessageContent { get; set; }
    
    [ProtoMember(11)]
    public ChangeTeamResultMessageContent? changeTeamResultMessageContent { get; set; }
}