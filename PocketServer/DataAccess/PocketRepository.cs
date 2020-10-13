using PocketServer.DataAccess.Core;

namespace PocketServer.DataAccess
{
    public class PocketRepository<T> : BaseRepository<T, DatabaseContext> where T : class
    {
        public PocketRepository(DatabaseContext context) : base(context) { }
    }
}
