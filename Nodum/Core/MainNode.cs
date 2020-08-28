namespace Nodum.Core
{
    public class MainNode : Node
    {
        public override bool IsEditable => true;

        public MainNode(string name = "MainNode") : base(name)
        {
        }
    }
}