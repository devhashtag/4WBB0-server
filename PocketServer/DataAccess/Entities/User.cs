using PocketServer.DataAccess.EntityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PocketServer.DataAccess.Entities
{
    public class User : IEntity<string>
    {
        public string Id { get; set; }

        public virtual IList<Subscription> Subscriptions { get; set; }
        public virtual IList<AlertResponse> AlertResponses { get; set; }

        public User()
        {
            Id = Guid.NewGuid().ToString();
            Subscriptions = new List<Subscription>();
            AlertResponses = new List<AlertResponse>();
        }
    }
}
