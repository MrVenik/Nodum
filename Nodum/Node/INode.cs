using System;

namespace Nodum.Node
{
    public interface INode
    {
        Position Position { get; set; }
        Guid Guid { get; }
        INodeHolder Holder { get; }
        string Name { get; set; }
        void Close();
        bool Showed { get; set; }
    }
}
