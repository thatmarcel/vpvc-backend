using VPVC_Backend.Shared.ProtobufMessages;

namespace VPVC_Backend.Interfaces; 

public interface IMessageHandler {
    public void HandleMessage(SessionMessage message, Guid senderSocketId);
}