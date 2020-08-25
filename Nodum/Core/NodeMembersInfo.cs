using System.Collections.Generic;
using System.Reflection;

namespace Nodum.Core
{
    public class NodeMembersInfo
    {
        public List<FieldInfo> FieldInfoList { get; } = new List<FieldInfo>();
        public List<PropertyInfo> PropertyInfoList { get; } = new List<PropertyInfo>();
    }
}
