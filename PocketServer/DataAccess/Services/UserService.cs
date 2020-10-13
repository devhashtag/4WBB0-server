using PocketServer.DataAccess.Core;
using PocketServer.DataAccess.Entities;

namespace PocketServer.DataAccess.Services
{
    public class UserService : BaseService<User, string>
    {
        public UserService(IRepository<User> repository): base(repository) { }
    }
}
