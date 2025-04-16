using System.Collections.Concurrent;

namespace SecureChat.Server.Models
{
    public class Room
    {
        public string Roomname { get; set; }
        public byte[] PublicKey { get; set; }
        public byte[] PrvateKey { get; set; }

        public ConcurrentDictionary<string, User> Users { get; } = new ConcurrentDictionary<string, User>();
    }
}
