using ProtoBuf;

namespace VPVC_Backend.Shared.ProtobufMessages.ClientToServer; 

[ProtoContract]
public class PartyJoinMessageContent {
    [ProtoMember(1)]
    public string? partyJoinCode { get; set; }
    
    [ProtoMember(2)]
    public string? userDisplayName { get; set; }
}