namespace SecureChat.Server.Models
{
    public class User
    {
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public byte[] PublicKey { get; set; }
    }
}
