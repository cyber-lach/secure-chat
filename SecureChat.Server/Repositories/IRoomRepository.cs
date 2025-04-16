using SecureChat.Server.Models;

namespace SecureChat.Server.Repositories
{
    public interface IRoomRepository
    {
        public Task<IList<Room>> All(CancellationToken cancellationToken);
        public Task<IList<Room>> AllForUser(string username, CancellationToken cancellationToken);
        public Task<Room?> Get(string roomname, CancellationToken cancellationToken);
        public Task<bool> Add(Room room, CancellationToken cancellationToken);
        public Task<bool> Delete(string roomname, CancellationToken cancellationToken);
        public Task<IList<User>> ListUsers(string roomname, CancellationToken cancellationToken);
        public Task<bool> AddUser(string roomname, User user, CancellationToken cancellationToken);
        public Task<bool> DeleteUser(string roomname, User user, CancellationToken cancellationToken);
    }
}
