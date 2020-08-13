namespace Nodum.Node
{
    public class NodePin
    {
        public INode Node { get; set; }
        public Position Position { get; set; }
        public string ElementId { get; set; }
        public bool Showed { get; set; }
    }
}