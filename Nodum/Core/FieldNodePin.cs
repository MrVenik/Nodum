using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Nodum.Core
{
    [Serializable]
    public class FieldNodePin<T> : NodePin<T>
    {
        [NonSerialized]
        private FieldInfo _fieldInfo;
        public FieldInfo FieldInfo { get => _fieldInfo; private set => _fieldInfo = value; }
        public string FieldInfoObjectTypeName { get; private set; }

        public override void SetNodeValue(Node node)
        {
            FieldInfo.SetValue(node, Value);
        }

        public override void GetNodeValue(Node node)
        {
            Value = (T)FieldInfo.GetValue(node);
        }

        public FieldNodePin(FieldInfo fieldInfo, Node node) : base(fieldInfo.Name, node, fieldInfo.GetCustomAttributes(true))
        {
            FieldInfo = fieldInfo;
            FieldInfoObjectTypeName = node.GetType().FullName;
        }

        [OnDeserialized]
        private void SetFieldInfo(StreamingContext context)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Type type = null;
            foreach (var assembly in assemblies)
            {
                type = assembly.GetType(FieldInfoObjectTypeName);
                if (type != null)
                {
                    break;
                }
            }

            if (type != null)
            {
                FieldInfo = type.GetField(Name);
            }
        }
    }
}
