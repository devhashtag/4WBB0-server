using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PocketServer.DataAccess.EntityTypes;

namespace PocketServer.DataAccess.Entities
{
    public class Device : IEntity<string>
    {
        public string Id { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerLastName { get; set; }
        public string OwnerAddress { get; set; }
        public string OwnerZipCode { get; set; }
        public string OwnerCity { get; set; }

        public IList<Subscription> Subscriptions { get; set; }
        public IList<Alert> Alerts { get; set; }
        public IList<Heartbeat> Heartbeats { get; set; }

        public Device()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
