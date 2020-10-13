using PocketServer.DataAccess.EntityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PocketServer.DataAccess.Entities
{
    public class Alert : IEntity<int>
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public bool Cancelled { get; set; }
        public DateTime Timestamp { get; set; }
        public double LocationLong { get; set; }
        public double LocationLat { get; set; }

        public Device Device { get; set; }
        public IList<AlertResponse> AlertResponses {get; set;}
    }
}
