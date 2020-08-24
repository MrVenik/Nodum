namespace Nodum.Core
{
    public class NodePinOptions
    {
        public bool IsInput { get; set; }
        public bool IsOutput { get; set; }
        public bool IsInternalInput { get; set; }
        public bool IsInternalOutput { get; set; }
        public bool IsInvokeUpdate { get; set; }
        public bool IsInvokeUpdatePins { get; set; }
        public bool CanSetValue { get; set; }
        public bool CanGetValue { get; set; }
    }
}
