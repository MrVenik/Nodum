using System;

namespace Nodum.Node
{
    public interface INode
    {
        Guid Guid { get; }
        string Name { get; set; }
        void Close();
    }
}
