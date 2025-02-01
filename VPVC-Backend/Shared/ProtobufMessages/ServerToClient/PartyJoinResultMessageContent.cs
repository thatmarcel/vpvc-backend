﻿using ProtoBuf;

namespace VPVC_Backend.Shared.ProtobufMessages.ServerToClient; 

[ProtoContract]
public class PartyJoinResultMessageContent {
    [ProtoMember(1)]
    public bool success { get; set; }
    
    [ProtoMember(2)]
    public SerializablePartyParticipant? partyParticipantSelf { get; set; }
    
    [ProtoMember(3)]
    public List<SerializablePartyParticipant>? partyParticipants { get; set; }
    
    [ProtoMember(4)]
    public byte[]? voiceChatEncryptionKey { get; set; }
}