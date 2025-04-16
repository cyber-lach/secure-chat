using SecureChat.Server.Models;

namespace SecureChat.Server.Repositories
{
    public interface IUserRepository
    {
        public Task<IList<User>> All(CancellationToken cancellationToken = default);
        public Task<User?> Get(string username, CancellationToken cancellationToken = default);
        public Task<User?> GetForConnection(string connectionId, CancellationToken cancellationToken = default);
        public Task<bool> Add(User user, CancellationToken cancellationToken = default);
        public Task<bool> Delete(string username, CancellationToken cancellationToken = default);
    }
}
