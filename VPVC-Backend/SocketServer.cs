using System.Net;
using System.Net.Sockets;
using NetCoreServer;

namespace VPVC_Backend; 

public class SocketServer : WsServer {
    public SocketServer(IPAddress address, int port) : base(address, port) { }
    public SocketServer(string address, int port) : base(address, port) { }
    public SocketServer(DnsEndPoint endpoint) : base(endpoint) { }
    public SocketServer(IPEndPoint endpoint) : base(endpoint) { }

    protected override TcpSession CreateSession() {
        return new SocketSession(this);
    }

    protected override void OnError(SocketError error) {
        Logger.Log($"SocketServer encountered an error with code {error}");
    }
}