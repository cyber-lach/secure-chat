using System.Collections.Concurrent;
using SecureChat.Server.Models;

namespace SecureChat.Server.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<string, User> _users;

        public UserRepository()
        {
            this._users = new ConcurrentDictionary<string, User>();
        }

        public Task<IList<User>> All(CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IList<User>)this._users.Values);
        }

        public Task<User?> Get(string username, CancellationToken cancellationToken = default) => Task.FromResult(_users.ContainsKey(username) ? _users[username] : null);

        public Task<User?> GetForConnection(string connectionId, CancellationToken cancellationToken = default) => Task.FromResult(_users.SingleOrDefault(x => x.Value.ConnectionId == connectionId).Value ?? null);

        public Task<bool> Add(User user, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_users.TryAdd(user.Name, user));
        }

        public Task<bool> Delete(string username, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_users.TryRemove(username, out var value));
        }
    }
}
