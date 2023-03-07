using VPVC_Backend.Interfaces;
using VPVC_Backend.Shared.ProtobufMessages;

namespace VPVC_Backend.MessageHandlers; 

public class SelfStateUpdateMessageHandler: IMessageHandler {
    public void HandleMessage(SessionMessage message, Guid senderSocketId) {
        if (message.selfStateUpdateMessageContent == null) {
            return;
        }

        var selfStateUpdateMessageContent = message.selfStateUpdateMessageContent;

        var party = Parties.PartyForSocketId(senderSocketId);
        var partyParticipant = party?.participants.FirstOrDefault(participant => participant.socketId == senderSocketId);

        if (partyParticipant == null) {
            return;
        }

        partyParticipant.gameState = selfStateUpdateMessageContent.gameState;
        partyParticipant.relativePositionX = selfStateUpdateMessageContent.relativePositionX;
        partyParticipant.relativePositionY = selfStateUpdateMessageContent.relativePositionY;
    }
}