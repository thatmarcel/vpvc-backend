using ProtoBuf;
using VPVC_Backend.Shared.ProtobufMessages;

namespace VPVC_Backend; 

// Class handling encoding and sending messages to clients
public static class MessageSender {
    public static void SendMessage(SessionMessage message, Guid socketId) {
        if (!SocketSessions.all.ContainsKey(socketId)) {
            return;
        }
        
        byte[] serializedMessageBytes;
        
        using (var stream = new MemoryStream()) {
            // Serialize the message to ProtoBuf data with a length prefix
            // as without one, de-serializing the data doesn't work
            // (presumably because of the padding added by the TCP socket library?)
            Serializer.SerializeWithLengthPrefix(stream, message, PrefixStyle.Base128, 1);
            serializedMessageBytes = stream.ToArray();
        }

        SocketSessions.all[socketId].SendBinaryAsync(serializedMessageBytes);
    }
}