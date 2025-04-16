namespace SecureChat.Client.Models
{
    public class User
    {
        public string Name { get; set; }
        public byte[] PublicKey { get; set; }
        public byte[] SharedSecret { get; set; }

        public List<Message> PrivateMessages { get; set; } = new List<Message>();
    }
}
