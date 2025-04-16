using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SecureChat.Server.Models;
using SecureChat.Server.Repositories;

namespace SecureChat.Server.Hubs
{
    public class ChatHub : Hub
    {
        //private readonly byte[] _privateKey;
        //public readonly byte[] PublicKey;
        private readonly IUserRepository _users;
        private readonly IRoomRepository _rooms;

        public ChatHub([FromServices] IUserRepository users, [FromServices] IRoomRepository rooms)
        {
            _users = users;
            _rooms = rooms;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendGroupMessage(string groupName, string message)
        {
            await Clients.Groups(groupName).SendAsync("receiveGroupMessage", message, _users.GetForConnection(Context.ConnectionId, CancellationToken.None), null, groupName);
        }

        public async Task AddRoomAsync(string roomname)
        {
            var room = new Models.Room { Roomname = roomname };
            await _rooms.Add(room, CancellationToken.None);

            await Clients.All.SendAsync("roomAdded", roomname, room.PublicKey);

            await GetRoomsAsync();
        }

        public async Task GetRoomsAsync()
        {
            await Clients.All.SendAsync("rooms", _rooms.All(CancellationToken.None));
        }

        public async Task SendUserMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveUserMessage", user, message);
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"{Context.ConnectionId} connected");
            return base.OnConnectedAsync();
        }
        public async Task Connect(string username, byte[] publicKey)
        {
            var user = new User
            {
                Name = username,
                ConnectionId = Context.ConnectionId,
                PublicKey = publicKey
            };

            var result = await _users.Add(user);
            if (result)
            {
                Console.WriteLine($"Added user {user.Name}");
                await Clients.Others.SendAsync("UserJoined", username, publicKey);
                await Clients.Caller.SendAsync("UserList", _users.All());
            }
        }

        public async Task Handshake(string username)
        {
            var recipient = await _users.Get(username) ?? throw new Exception("Recipient user not found!");
            var sender = await _users.GetForConnection(Context.ConnectionId) ?? throw new Exception("Sender user not found!");

            await Clients.Client(recipient.ConnectionId).SendAsync("IncomingHandshake", sender.Name);
        }

        public async Task PrivateMessage(string username, byte[] encryptedMessageBytes)
        {
            var recipient = await _users.Get(username) ?? throw new Exception("Recipient user not found!");
            var sender = await _users.GetForConnection(Context.ConnectionId) ?? throw new Exception("Sender user not found!");

            await Clients.Client(recipient.ConnectionId).SendAsync("IncomingPrivateMessage", sender.Name, encryptedMessageBytes);
        }


        public override async Task OnDisconnectedAsync(Exception? e)
        {
            Console.WriteLine($"Disconnected {e?.Message} {Context.ConnectionId}");
            var user = await _users.GetForConnection(Context.ConnectionId, CancellationToken.None);
            if (user != null)
            {
                if (await _users.Delete(user.Name))
                {
                    Console.WriteLine($"Removed user {user.Name}");
                }
            }

            await base.OnDisconnectedAsync(e);
        }
    }
}
