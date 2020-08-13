namespace Nodum.Node
{
    public interface IInputNode : IValueNode
    {
        Connection[] GetIncomingConnections();
    }
}