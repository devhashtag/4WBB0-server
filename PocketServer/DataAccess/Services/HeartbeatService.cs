using PocketServer.DataAccess.Core;
using PocketServer.DataAccess.Entities;

namespace PocketServer.DataAccess.Services
{
    public class HeartbeatService : BaseService<Heartbeat, int>
    {
        public HeartbeatService(IRepository<Heartbeat> repository) : base(repository) { }
    }
}
