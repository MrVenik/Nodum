using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nodum.Core
{
    public static class NodeBuilder
    {
        private static Dictionary<Type, NodeMembersInfo> _nodeInfoList;
        public static List<Node> AllBaseNodes { get; } = new List<Node>();

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

        public static NodeMembersInfo GetNodeMembers(Type type)
        {
            if (_nodeInfoList.ContainsKey(type))
            {
                return _nodeInfoList[type];
            }
            return null;
        }

        public static void CacheBaseNodes()
        {
            if (_nodeInfoList == null)
            {
                _nodeInfoList = new Dictionary<Type, NodeMembersInfo>();

                Type baseType = typeof(Node);
                List<Type> nodeTypes = new List<Type>();
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (Assembly assembly in assemblies)
                {
                    string assemblyName = assembly.GetName().Name;
                    int index = assemblyName.IndexOf('.');
                    if (index != -1) assemblyName = assemblyName.Substring(0, index);
                    switch (assemblyName)
                    {
                        case "System":
                        case "mscorlib":
                        case "Microsoft":
                            continue;
                        default:
                            nodeTypes.AddRange(assembly.GetTypes().Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t)).ToArray());
                            break;
                    }
                }

                foreach (Type type in nodeTypes)
                {
                    CacheNodeMembers(type);
                    Node node = (Node)Activator.CreateInstance(type, type.Name);
                    AllBaseNodes.Add(node);
                }
            }
        }

        public static Node CloneNode(Node node)
        {
            if (node != null)
            {
                BinaryNodeSerializer serializer = new BinaryNodeSerializer();
                byte[] bytes = serializer.SerializeToByteArray(node);
                return serializer.DeserializeFromByteArray(bytes);
            }
            throw new Exception("Node is null");
        }

        private static void CacheNodeMembers(Type nodeType)
        {
            CacheNodeFields(nodeType);
            CacheNodeProperties(nodeType);
        }

        private static void CacheNodeProperties(Type nodeType)
        {
            PropertyInfo[] propertyInfos = nodeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in propertyInfos)
            {
                if (property.CanWrite && property.CanRead)
                {
                    object[] attribs = property.GetCustomAttributes(true);
                    if (attribs.FirstOrDefault(x => x is Node.NodePinAttribute) is Node.NodePinAttribute nodePinAttribete)
                    {
                        if (!_nodeInfoList.ContainsKey(nodeType))
                        {
                            _nodeInfoList.Add(nodeType, new NodeMembersInfo());
                        }
                        _nodeInfoList[nodeType].PropertyInfoList.Add(property);
                    }
                }
            }
        }

        private static void CacheNodeFields(Type nodeType)
        {
            FieldInfo[] fieldInfos = nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fieldInfos)
            {
                object[] attribs = field.GetCustomAttributes(true);
                if (attribs.FirstOrDefault(x => x is Node.NodePinAttribute) is Node.NodePinAttribute nodePinAttribete)
                {
                    if (!_nodeInfoList.ContainsKey(nodeType))
                    {
                        _nodeInfoList.Add(nodeType, new NodeMembersInfo());
                    }
                    _nodeInfoList[nodeType].FieldInfoList.Add(field);
                }
            }
        }
    }
}
