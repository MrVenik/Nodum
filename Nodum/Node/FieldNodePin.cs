using System.Reflection;

namespace Nodum.Node
{
    public class FieldNodePin<T> : NodePin<T>
    {
        public FieldInfo FieldInfo { get; private set; }

        public override void SetNodeValue(Node node)
        {
            FieldInfo.SetValue(node, Value); 
        }

        public override void GetNodeValue(Node node)
        {
            Value = (T)FieldInfo.GetValue(node);
        }

        public FieldNodePin(FieldInfo fieldInfo) : base(fieldInfo.Name, fieldInfo.GetCustomAttributes(true))
        {
            FieldInfo = fieldInfo;
        }
    }
}
