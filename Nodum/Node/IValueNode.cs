using System;

namespace Nodum.Node
{
    public interface IValueNode : IOutputNode
    {
        object Value { get; set; }
        Type ValueType { get; }
        Action OnValueChanged { get; set; }
        void UpdateValue();
    }



    public interface IValueNode<T> : IValueNode
    {
        new T Value { get; set; }
    }
}