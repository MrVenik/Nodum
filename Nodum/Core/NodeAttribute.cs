using System;

namespace Nodum.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeAttribute : Attribute
    {
        public string Group { get; set; }
        public bool NodeCacherIgnore { get; set; }
    }
}
