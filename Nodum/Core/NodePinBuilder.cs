using Nodum.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Nodum.Core
{
    public static class NodePinBuilder
    {
        public static NodePin BuildNodePin(FieldInfo fieldInfo, Node node)
        {
            Type type = typeof(FieldNodePin<>);
            Type genericType = type.MakeGenericType(fieldInfo.FieldType);

            NodePin nodePin = (NodePin)Activator.CreateInstance(genericType, new object[] { fieldInfo, node });

            return nodePin;
        }

        public static NodePin BuildNodePin(PropertyInfo propertyInfo, Node node)
        {
            Type type = typeof(PropertyNodePin<>);
            Type genericType = type.MakeGenericType(propertyInfo.PropertyType);

            NodePin nodePin = (NodePin)Activator.CreateInstance(genericType, new object[] { propertyInfo, node });

            return nodePin;
        }

        public static NodePin BuildNodePin(string name, Node node, Type valueType, NodePinOptions options)
        {
            Type type = typeof(NodePin<>);
            Type genericType = type.MakeGenericType(valueType);

            object[] parameters = new object[] { name, node, options };

            NodePin nodePin = (NodePin)Activator.CreateInstance(genericType, parameters);

            return nodePin;
        }
    }
}
