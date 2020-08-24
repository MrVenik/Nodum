using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Nodum.Core
{
    [Serializable]
    public class PropertyNodePin<T> : NodePin<T>
    {
        [NonSerialized]
        private PropertyInfo _propertyInfo;
        public PropertyInfo PropertyInfo { get => _propertyInfo; private set => _propertyInfo = value; }
        public string PropertyInfoObjectTypeName { get; private set; }


        public override void SetNodeValue(Node node)
        {
            PropertyInfo.SetValue(node, Value);
        }

        public override void GetNodeValue(Node node)
        {
            Value = (T)PropertyInfo.GetValue(node);
        }

        public PropertyNodePin(PropertyInfo propertyInfo, Node node) : base(propertyInfo.Name, node, propertyInfo.GetCustomAttributes(true))
        {
            PropertyInfo = propertyInfo;
            PropertyInfoObjectTypeName = node.GetType().FullName;
        }

        [OnDeserialized]
        private void SetPropertyInfo(StreamingContext context)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Type type = null;
            foreach (var assembly in assemblies)
            {
                type = assembly.GetType(PropertyInfoObjectTypeName);
                if (type != null)
                {
                    break;
                }
            }

            if (type != null)
            {
                PropertyInfo = type.GetProperty(Name);
            }
        }
    }
}
