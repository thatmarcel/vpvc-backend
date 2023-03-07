using System.Net.Sockets;
using NetCoreServer;
using ProtoBuf;
using VPVC_Backend.Shared.ProtobufMessages;

namespace VPVC_Backend; 

public class SocketSession : WsSession {
    public SocketSession(WsServer server) : base(server) { }

    public override void OnWsConnected(HttpRequest request) {
        Logger.LogVerbose($"SocketSession (id: {Id}) connected");
        
        SocketSessions.all[Id] = this;
    }

    protected override void OnDisconnected() {
        Logger.LogVerbose($"SocketSession (id: {Id}) disconnected");
        
        Parties.LeaveParty(Id);
        SocketSessions.all.Remove(Id);
    }

    public override void OnWsReceived(byte[] buffer, long offset, long size) {
        try {
            SessionMessage message = Serializer.DeserializeWithLengthPrefix<SessionMessage>(new MemoryStream(buffer), PrefixStyle.Base128, 1);

            MessageReceiver.MessageReceived(message, Id);
        } catch (Exception exception) {
            Logger.Log(exception.ToString());
        }
    }

    protected override void OnError(SocketError error) {
        Logger.Log($"SocketSession (id: {Id}) encountered an error with code {error}");
        
        SocketSessions.all.Remove(Id);
    }
}