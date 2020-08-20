using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nodum.Node
{
    public static class NodeBuilder
    {
        private static Dictionary<Type, List<FieldInfo>> _nodeFields;
        private static Dictionary<Type, List<PropertyInfo>> _nodeProperties;
        private static bool FieldsInitialized => _nodeFields != null;
        private static bool PropertiesInitialized => _nodeProperties != null;
        private static bool Initialized => FieldsInitialized && PropertiesInitialized;

        public static NodePin BuildNodePin(FieldInfo fieldInfo)
        {
            Type type = typeof(FieldNodePin<>);
            Type genericType = type.MakeGenericType(fieldInfo.FieldType);

            NodePin nodePin = (NodePin)Activator.CreateInstance(genericType, new object[] { fieldInfo });

            return nodePin;
        }

        public static NodePin BuildNodePin(PropertyInfo propertyInfo)
        {
            Type type = typeof(PropertyNodePin<>);
            Type genericType = type.MakeGenericType(propertyInfo.PropertyType);

            NodePin nodePin = (NodePin)Activator.CreateInstance(genericType, new object[] { propertyInfo });

            return nodePin;
        }

        public static NodePin BuildNodePin(string name, Type valueType, bool isInput = false, bool isOutput = false, bool isInvokeUpdate = false)
        {
            Type type = typeof(NodePin<>);
            Type genericType = type.MakeGenericType(valueType);

            NodePin nodePin = (NodePin)Activator.CreateInstance(genericType, new object[] { name, isInput, isOutput, isInvokeUpdate });

            return nodePin;
        }

        public static void BuildNode(Node node)
        {
            if (!Initialized) BuildCache();

            Type nodeType = node.GetType();

            if (_nodeFields.TryGetValue(nodeType, out List<FieldInfo> typeNodeFields))
            {
                for (int i = 0; i < typeNodeFields.Count; i++)
                {
                    FieldInfo fieldInfo = typeNodeFields[i];

                    NodePin nodePin = BuildNodePin(fieldInfo);

                    node.AddNodePin(nodePin);
                }
            }

            if (_nodeProperties.TryGetValue(nodeType, out List<PropertyInfo> typeNodeProperties))
            {
                for (int i = 0; i < typeNodeProperties.Count; i++)
                {
                    PropertyInfo propertyInfo = typeNodeProperties[i];

                    NodePin nodePin = BuildNodePin(propertyInfo);

                    node.AddNodePin(nodePin);
                }
            }
        }

        private static void BuildCache()
        {
            _nodeFields = new Dictionary<Type, List<FieldInfo>>();
            _nodeProperties = new Dictionary<Type, List<PropertyInfo>>();

            Type baseType = typeof(Node);
            List<Type> nodeTypes = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Loop through assemblies and add node types to list
            foreach (Assembly assembly in assemblies)
            {
                // Skip certain dlls to improve performance
                string assemblyName = assembly.GetName().Name;
                int index = assemblyName.IndexOf('.');
                if (index != -1) assemblyName = assemblyName.Substring(0, index);
                switch (assemblyName)
                {
                    // The following assemblies, and sub-assemblies (eg. UnityEngine.UI) are skipped
                    case "System":
                    case "mscorlib":
                    case "Microsoft":
                        continue;
                    default:
                        nodeTypes.AddRange(assembly.GetTypes().Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t)).ToArray());
                        break;
                }
            }

            for (int i = 0; i < nodeTypes.Count; i++)
            {
                CachePorts(nodeTypes[i]);
            }
        }

        private static void CachePorts(Type nodeType)
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
                        if (!_nodeProperties.ContainsKey(nodeType))
                        {
                            _nodeProperties.Add(nodeType, new List<PropertyInfo>());
                        }
                        _nodeProperties[nodeType].Add(property);
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
                    if (!_nodeFields.ContainsKey(nodeType))
                    {
                        _nodeFields.Add(nodeType, new List<FieldInfo>());
                    }
                    _nodeFields[nodeType].Add(field);
                }
            }
        }
    }
}
