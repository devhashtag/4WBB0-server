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

        public IList<Subscription> Subscriptions { get; set; }
        public IList<AlertResponse> AlertResponses { get; set; }

        public User()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
