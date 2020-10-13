using PocketServer.DataAccess.EntityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PocketServer.DataAccess.Entities
{
    public class Heartbeat : IEntity<int>
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public double Longitude { get; set; }
        public double Lattitue { get; set; }

        public Device Device { get; set; }
    }
}
