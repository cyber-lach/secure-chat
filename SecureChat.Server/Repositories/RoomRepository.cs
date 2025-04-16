using System.Collections.Concurrent;
using SecureChat.Server.Models;

namespace SecureChat.Server.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ConcurrentDictionary<string, Room> _rooms;

        public RoomRepository()
        {
            this._rooms = new ConcurrentDictionary<string, Room>();
        }

        public Task<IList<Room>> All(CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IList<Room>)this._rooms.Values);
        }

        public Task<IList<Room>> AllForUser(string username, CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IList<Room>)this._rooms.Where(x => x.Value.Users.ContainsKey(username)).Select(x => x.Value));
        }

        public Task<Room?> Get(string roomname, CancellationToken cancellationToken = default) => Task.FromResult(_rooms.ContainsKey(roomname) ? _rooms[roomname] : null);

        public Task<bool> Add(Room room, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_rooms.TryAdd(room.Roomname, room));
        }

        public Task<bool> Delete(string roomname, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_rooms.TryRemove(roomname, out var value));
        }

        public Task<IList<User>> ListUsers(string roomname, CancellationToken cancellationToken)
        {
            var room = Get(roomname, cancellationToken).Result;
            if (room == null)
            {
                return Task.FromResult((IList<User>)[]);
            }

            return Task.FromResult((IList<User>)room.Users.Values);
        }

        public Task<bool> AddUser(string roomname, User user, CancellationToken cancellationToken = default)
        {
            var room = Get(roomname, cancellationToken).Result;
            if (room == null) return Task.FromResult(false);

            return Task.FromResult(room.Users.TryAdd(user.Name, user));
        }

        public Task<bool> DeleteUser(string roomname, User user, CancellationToken cancellationToken = default)
        {
            var room = Get(roomname, cancellationToken).Result;
            if (room == null) return Task.FromResult(false);

            return Task.FromResult(room.Users.TryRemove(user.Name, out var value));
        }
    }
}
