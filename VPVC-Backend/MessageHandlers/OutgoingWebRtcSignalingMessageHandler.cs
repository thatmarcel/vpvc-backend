using VPVC_Backend.Interfaces;
using VPVC_Backend.Shared;
using VPVC_Backend.Shared.ProtobufMessages;
using VPVC_Backend.Shared.ProtobufMessages.ServerToClient;

namespace VPVC_Backend.MessageHandlers; 

public class OutgoingWebRtcSignalingMessageHandler: IMessageHandler {
    public void HandleMessage(SessionMessage message, Guid senderSocketId) {
        if (message.outgoingWebRtcSignalingMessageContent == null) {
            return;
        }

        var outgoingWebRtcSignalingMessageContent = message.outgoingWebRtcSignalingMessageContent;

        var party = Parties.PartyForSocketId(senderSocketId);
        var sendingPartyParticipant = party?.participants.FirstOrDefault(participant => participant.socketId == senderSocketId);
        var receivingPartyParticipant = party?.participants.FirstOrDefault(participant => participant.id == outgoingWebRtcSignalingMessageContent.receivingParticipantId);

        if (sendingPartyParticipant == null || receivingPartyParticipant == null) {
            return;
        }

        var resultMessage = new SessionMessage {
            type = MessageTypes.incomingWebRtcSignalingMessage,
            incomingWebRtcSignalingMessageContent = new IncomingWebRtcSignalingMessageContent {
                sendingParticipantId = sendingPartyParticipant.id,
                signalingMessageType = outgoingWebRtcSignalingMessageContent.signalingMessageType,
                sdpContent = outgoingWebRtcSignalingMessageContent.sdpContent
            }
        };
            
        MessageSender.SendMessage(resultMessage, receivingPartyParticipant.socketId);
    }
}