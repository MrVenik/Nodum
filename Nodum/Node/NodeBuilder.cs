using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nodum.Node
{
    public static class NodeBuilder
    {
        private static Dictionary<Type, List<FieldInfo>> _nodeFields;
        private static bool Initialized { get { return _nodeFields != null; } }

        public static NodePin BuildNodePin(FieldInfo fieldInfo)
        {
            Type type = typeof(NodePin<>);
            Type genericType = type.MakeGenericType(fieldInfo.FieldType);

            NodePin nodePin = (NodePin)Activator.CreateInstance(genericType, new object[] { fieldInfo });

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
        }

        private static void BuildCache()
        {
            _nodeFields = new Dictionary<Type, List<FieldInfo>>();

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

        public static List<FieldInfo> GetNodeFields(Type nodeType)
        {
            List<FieldInfo> fieldInfo = new List<FieldInfo>(nodeType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

            Type tempType = nodeType;

            FieldInfo[] parentFields = nodeType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < parentFields.Length; i++)
            {
                FieldInfo parentField = parentFields[i];
                if (fieldInfo.TrueForAll(x => x.Name != parentField.Name))
                {
                    fieldInfo.Add(parentField);
                }
            }

            return fieldInfo;
        }

        private static void CachePorts(Type nodeType)
        {
            List<FieldInfo> fieldInfo = GetNodeFields(nodeType);

            for (int i = 0; i < fieldInfo.Count; i++)
            {
                object[] attribs = fieldInfo[i].GetCustomAttributes(true);

                if (attribs.FirstOrDefault(x => x is Node.NodePinAttribute) is Node.NodePinAttribute nodePinAttribete)
                {
                    if (!_nodeFields.ContainsKey(nodeType))
                    {
                        _nodeFields.Add(nodeType, new List<FieldInfo>());
                    }
                    _nodeFields[nodeType].Add(fieldInfo[i]);
                }
            }
        }
    }
}
