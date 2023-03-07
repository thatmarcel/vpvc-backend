using VPVC_Backend.Interfaces;
using VPVC_Backend.Shared;
using VPVC_Backend.Shared.ProtobufMessages;
using VPVC_Backend.Shared.ProtobufMessages.ServerToClient;

namespace VPVC_Backend.MessageHandlers; 

public class ChangeTeamMessageHandler: IMessageHandler {
    public void HandleMessage(SessionMessage message, Guid senderSocketId) {
        if (message.changeTeamMessageContent == null) {
            return;
        }

        var changeTeamMessageContent = message.changeTeamMessageContent;

        var changeTeamResult = Parties.ChangeTeam(senderSocketId, changeTeamMessageContent.newTeamIndex);
        var success = changeTeamResult.Item1;
        var newTeamIndex = changeTeamResult.Item2;
        
        var resultMessage = new SessionMessage {
            type = MessageTypes.changeTeamResult,
            changeTeamResultMessageContent = new ChangeTeamResultMessageContent {
                success = success,
                newTeamIndex = newTeamIndex
            }
        };
            
        MessageSender.SendMessage(resultMessage, senderSocketId);
    }
}