using VPVC_Backend.Interfaces;
using VPVC_Backend.Shared;
using VPVC_Backend.Shared.ProtobufMessages;
using VPVC_Backend.Shared.ProtobufMessages.ClientToServer;
using VPVC_Backend.Shared.ProtobufMessages.ServerToClient;

namespace VPVC_Backend.MessageHandlers; 

public class PartyCreateMessageHandler: IMessageHandler {
    public void HandleMessage(SessionMessage message, Guid senderSocketId) {
        if (message.partyCreateMessageContent == null) {
            return;
        }

        var partyCreateMessageContent = message.partyCreateMessageContent;

        if (partyCreateMessageContent.userDisplayName == null) {
            SendErrorMessage(senderSocketId);
            return;
        }

        var party = Parties.CreateAndJoinParty(senderSocketId, partyCreateMessageContent.userDisplayName);

        if (party == null || party.participants.Count < 1) {
            SendErrorMessage(senderSocketId);
            return;
        }

        var resultMessage = new SessionMessage {
            type = MessageTypes.partyCreateResult,
            partyCreateResultMessageContent = new PartyCreateResultMessageContent {
                success = true,
                partyJoinCode = party.joinCode,
                partyParticipantSelf = party.participants[0].ToSerializable(),
                voiceChatEncryptionKey = party.voiceChatEncryptionKey
            }
        };
        
        MessageSender.SendMessage(resultMessage, senderSocketId);
    }

    private void SendErrorMessage(Guid socketId) {
        var errorMessage = new SessionMessage {
            type = MessageTypes.partyCreateResult,
            partyCreateResultMessageContent = new PartyCreateResultMessageContent {
                success = false
            }
        };
            
        MessageSender.SendMessage(errorMessage, socketId);
    }
}