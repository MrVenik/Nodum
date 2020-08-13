using System.Collections.Generic;

namespace Nodum.Node
{
    public class CommutateInputNodePin : NodePin
    {
        public List<IOutputNode> IncomingNodes { get; set; }
        public List<Connection> IncomingConnections { get; set; }
    }
}