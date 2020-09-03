using Nodum.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static NodePin CloneNodePin(NodePin originalNodePin, Node node)
        {
            Type type = originalNodePin.GetType();

            NodePinOptions options = new NodePinOptions()
            {
                IsInput = originalNodePin.IsInput,
                IsOutput = originalNodePin.IsOutput,
                IsOption = originalNodePin.IsOption,
                IsInternalInput = originalNodePin.IsInternalInput,
                IsInternalOutput = originalNodePin.IsInternalOutput,
                IsInvokeUpdate = originalNodePin.IsInvokeUpdate,
                IsInvokeUpdatePins = originalNodePin.IsInvokeUpdatePins,
                CanSetValue = originalNodePin.CanSetValue,
                CanGetValue = originalNodePin.CanGetValue,
            };

            object[] parameters = new object[] { originalNodePin.Name, node, options };

            NodePin nodePin = (NodePin)Activator.CreateInstance(type, parameters);

            return nodePin;
        }

        public static NodePin BuildNodePin(FieldInfo fieldInfo, Node node)
        {
            CheckType(fieldInfo.FieldType);

            Type type = typeof(FieldNodePin<>);
            Type genericType = type.MakeGenericType(fieldInfo.FieldType);

            NodePin nodePin = (NodePin)Activator.CreateInstance(genericType, new object[] { fieldInfo, node });

            return nodePin;
        }

        public static NodePin BuildNodePin(PropertyInfo propertyInfo, Node node)
        {
            CheckType(propertyInfo.PropertyType);

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
            CheckType(valueType);

            Type type = typeof(NodePin<>);
            Type genericType = type.MakeGenericType(valueType);

            object[] parameters = new object[] { name, node, options };

            NodePin nodePin = (NodePin)Activator.CreateInstance(genericType, parameters);

            return nodePin;
        }

        public static NodePin BuildGenericNodePin(string name, Node node, Type genericValueType, NodePinOptions options, (string, Type)[] genericArguments)
        {
            CheckType(genericValueType);

            Type type = typeof(NodePin<>);
            Type genericType = type.MakeGenericType(MakeGenericType(genericValueType, genericArguments));

            object[] parameters = new object[] { name, node, options };

            NodePin nodePin = (NodePin)Activator.CreateInstance(genericType, parameters);

            return nodePin;
        }

        private static Type MakeGenericType(Type genericType, params (string, Type)[] genericArguments)
        {
            if (genericType.ContainsGenericParameters)
            {
                if (genericType.IsGenericType)
                {
                    List<Type> genericTypes = new List<Type>();
                    foreach (var item in genericType.GetGenericArguments())
                    {
                        genericTypes.Add(MakeGenericType(item, genericArguments));
                    }
                    foreach (var item in genericTypes)
                    {
                        genericType = genericType.GetGenericTypeDefinition().MakeGenericType(genericTypes.ToArray());
                    }
                }
                else
                {
                    if (genericType.IsArray)
                    {
                        Type arrayType = genericArguments.Where(a => a.Item1 == genericType.GetElementType().Name).FirstOrDefault().Item2;
                        genericType = arrayType.MakeArrayType();
                    }
                    else
                    {
                        genericType = genericArguments.Where(a => a.Item1 == genericType.Name).FirstOrDefault().Item2;
                    }
                }
            }

            return genericType;
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
