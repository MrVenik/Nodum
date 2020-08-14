namespace Nodum.Node
{
    public interface IInputNode : IValueNode
    {
        int AmountOfInputs { get; }
    }
}