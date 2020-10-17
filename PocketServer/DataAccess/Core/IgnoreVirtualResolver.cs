using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PocketServer.DataAccess.Core
{
    public class IgnoreVirtualResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var propertyInfo = member as PropertyInfo;

            if (propertyInfo != null)
            {
                if (propertyInfo.GetMethod.IsVirtual && !propertyInfo.GetMethod.IsFinal)
                {
                    property.ShouldSerialize = obj => false;
                }
            }

            return property;
        }
    }
}
