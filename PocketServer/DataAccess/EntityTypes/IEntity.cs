using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PocketServer.DataAccess.EntityTypes
{
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
}
