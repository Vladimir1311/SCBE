namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public class FromClientPack
    {
        public ApplicationUser User;
        public PackType PackType;
        public byte[] Data;
    }
}