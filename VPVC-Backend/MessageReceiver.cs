using System.Collections.Concurrent;
using VPVC_Backend.Interfaces;
using VPVC_Backend.MessageHandlers;
using VPVC_Backend.Shared;
using VPVC_Backend.Shared.ProtobufMessages;

namespace VPVC_Backend; 

// Class handling the forwarding of received messages to the correct message handler
// (messages are queued and processed one after another instead of directly when received
// to prevent problems caused by two messages being processed at the same time and both making
// modifications to stored variables which can e.g. lead to duplicate entries in lists)
public static class MessageReceiver {
    private static readonly Dictionary<int, IMessageHandler> messageHandlers = new() {
        { MessageTypes.partyJoin, new PartyJoinMessageHandler() },
        { MessageTypes.partyCreate, new PartyCreateMessageHandler() },
        { MessageTypes.selfStateUpdate, new SelfStateUpdateMessageHandler() },
        { MessageTypes.outgoingWebRtcSignalingMessage, new OutgoingWebRtcSignalingMessageHandler() },
        { MessageTypes.changeTeam, new ChangeTeamMessageHandler() }
    };

    // Stores the list of messages that have yet to be processed
    private static readonly BlockingCollection<Tuple<SessionMessage, Guid>> messageQueue = new();

    // When a message is received, add it to the queue for processing
    public static void MessageReceived(SessionMessage message, Guid senderSocketId) {
        messageQueue.Add(new Tuple<SessionMessage, Guid>(message, senderSocketId));
    }

    // Process the next message in the queue
    public static void ProcessNextMessage() {
        Tuple<SessionMessage, Guid> nextMessageInfo = messageQueue.Take();
        
        if (messageHandlers.ContainsKey(nextMessageInfo.Item1.type)) {
            messageHandlers[nextMessageInfo.Item1.type].HandleMessage(nextMessageInfo.Item1, nextMessageInfo.Item2);
        }
    }
}