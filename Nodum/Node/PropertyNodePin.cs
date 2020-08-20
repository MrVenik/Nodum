using System.Reflection;

namespace Nodum.Node
{
    public class PropertyNodePin<T> : NodePin<T>
    {
        public PropertyInfo PropertyInfo { get; private set; }


        public override void SetNodeValue(Node node)
        {
            PropertyInfo.SetValue(node, Value);
        }

        public override void GetNodeValue(Node node)
        {
            Value = (T)PropertyInfo.GetValue(node);
        }

        public PropertyNodePin(PropertyInfo propertyInfo) : base(propertyInfo.Name, propertyInfo.GetCustomAttributes(true))
        {
            PropertyInfo = propertyInfo;
        }
    }
}
