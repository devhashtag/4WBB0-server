using PocketServer.DataAccess.EntityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PocketServer.DataAccess.Entities
{
    public class AlertResponse : IEntity<int>
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int AlertId { get; set; }
        public DateTime Received { get; set; }
        public DateTime Seen { get; set; }

        public virtual User User { get; set; }
        public virtual Alert Alert { get; set; }
    }
}
