using PocketServer.DataAccess.Core;
using PocketServer.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PocketServer.DataAccess.Services
{
    public class DeviceService : BaseService<Device, string>
    {
        public DeviceService(IRepository<Device> repository) : base(repository) { }
    }
}
