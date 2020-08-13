namespace Nodum.Node
{
    public class InputNodePin : NodePin
    {
        public IOutputNode IncomingNode { get; set; }
        public Connection IncomingConnection { get; set; }
    }
}