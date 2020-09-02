namespace Nodum.Core
{
    public interface INodeCompiler
    {
        public void Compile(params Node[] nodes);
    }
}
