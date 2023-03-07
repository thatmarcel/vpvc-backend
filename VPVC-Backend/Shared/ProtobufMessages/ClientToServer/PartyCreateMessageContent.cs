using ProtoBuf;

namespace VPVC_Backend.Shared.ProtobufMessages.ClientToServer; 

[ProtoContract]
public class PartyCreateMessageContent {
    [ProtoMember(1)]
    public string? userDisplayName { get; set; }
}