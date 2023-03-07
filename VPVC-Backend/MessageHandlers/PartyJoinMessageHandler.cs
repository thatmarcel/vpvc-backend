using VPVC_Backend.Interfaces;
using VPVC_Backend.Shared;
using VPVC_Backend.Shared.ProtobufMessages;
using VPVC_Backend.Shared.ProtobufMessages.ClientToServer;
using VPVC_Backend.Shared.ProtobufMessages.ServerToClient;

namespace VPVC_Backend.MessageHandlers; 

public class PartyJoinMessageHandler: IMessageHandler {
    public void HandleMessage(SessionMessage message, Guid senderSocketId) {
        if (message.partyJoinMessageContent == null) {
            return;
        }

        var partyJoinMessageContent = message.partyJoinMessageContent;
        
        if (partyJoinMessageContent.partyJoinCode == null || partyJoinMessageContent.userDisplayName == null) {
            SendErrorMessage(senderSocketId);
            return;
        }

        var party = Parties.JoinParty(partyJoinMessageContent.partyJoinCode, senderSocketId, partyJoinMessageContent.userDisplayName);

        if (party == null) {
            SendErrorMessage(senderSocketId);
            return;
        }

        var partyParticipantSelf = party.participants.Last();
        var partyParticipants = new List<PartyParticipant>(party.participants);
        partyParticipants.Remove(partyParticipantSelf);
        
        var resultMessage = new SessionMessage {
            type = MessageTypes.partyJoinResult,
            partyJoinResultMessageContent = new PartyJoinResultMessageContent {
                success = true,
                partyParticipantSelf = partyParticipantSelf.ToSerializable(),
                partyParticipants = partyParticipants.Select(participant => participant.ToSerializable()).ToList()
            }
        };
            
        MessageSender.SendMessage(resultMessage, senderSocketId);
    }
    
    private void SendErrorMessage(Guid socketId) {
        var errorMessage = new SessionMessage {
            type = MessageTypes.partyJoinResult,
            partyJoinResultMessageContent = new PartyJoinResultMessageContent {
                success = false
            }
        };
            
        MessageSender.SendMessage(errorMessage, socketId);
    }
}