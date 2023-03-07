using System.Net;
using VPVC_Backend;

Logger.Log("Starting...");

int port = 5000;
SocketServer server = new SocketServer(IPAddress.Parse("127.0.0.1"), port);
server.Start();

Parties.SetupParticipantStatesUpdateTimer();

Logger.Log("Started.");

for (;;) {
    MessageReceiver.ProcessNextMessage();
}