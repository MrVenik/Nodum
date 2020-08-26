using Nodum.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Nodum.Core
{
    public static class NodePinBuilder
    {
        private static bool CheckType(Type type)
        {
            bool canBuildNodePin = true; 
            if (type.IsByRef)
            {
                throw new NodePinException($"NodePin can't be builded with ByRef type " + type.Name);
            }
            if (type.IsByRefLike)
            {
                throw new NodePinException($"NodePin can't be builded with ByRefLike type " + type.Name);
            }
            if (type.IsPointer)
            {
                throw new NodePinException($"NodePin can't be builded with Pointer type " + type.Name);
            }
            return canBuildNodePin;
        }

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

        public static NodePin BuildNodePin(ParameterInfo parameterInfo, Node node)
        {
            CheckType(parameterInfo.ParameterType);

            Type type = typeof(NodePin<>);
            Type genericType = type.MakeGenericType(parameterInfo.ParameterType);

            NodePinOptions options = new NodePinOptions()
            {
                IsInput = true,
                IsInvokeUpdate = true          
            };

            object[] parameters = new object[] { parameterInfo.Name, node, options };

            NodePin nodePin = (NodePin)Activator.CreateInstance(genericType, parameters);

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

        public static NodePin[] BuildNodePins(ParameterInfo[] parameters, Node node)
        {
            NodePin[] nodePins = new NodePin[parameters.Length];

            for (int i = 0; i < nodePins.Length; i++)
            {
                nodePins[i] = BuildNodePin(parameters[i], node);
            }

            return nodePins;
        }
    }
}
