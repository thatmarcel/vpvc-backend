namespace VPVC_Backend; 

public class Party {
    public string id;
    public string joinCode;
    public byte[] voiceChatEncryptionKey;

    public readonly List<PartyParticipant> participants = new();

    public Party(string id, string joinCode, byte[] voiceChatEncryptionKey) {
        this.id = id;
        this.joinCode = joinCode;
        this.voiceChatEncryptionKey = voiceChatEncryptionKey;
    }
}