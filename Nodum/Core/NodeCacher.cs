using Nodum.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nodum.Core
{
    public static class NodeCacher
    {
        private static Dictionary<Type, NodeMembersInfo> _nodeInfoList;

        private static Dictionary<string, List<Node>> _allBaseNodeGroups = new Dictionary<string, List<Node>>();

        public static IReadOnlyDictionary<string, List<Node>> AllBaseNodeGroups => _allBaseNodeGroups;

        public static NodeMembersInfo GetNodeMembers(Type type)
        {
            if (_nodeInfoList.ContainsKey(type))
            {
                return _nodeInfoList[type];
            }
            return null;
        }

        public static void CacheBaseNodes(Assembly assembly)
        {
            Type baseType = typeof(Node);

            List<Type> nodeTypes = new List<Type>();

            nodeTypes.AddRange(assembly.GetTypes().Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t)).ToArray());

            foreach (Type type in nodeTypes)
            {
                CacheNodeMembers(type);
                object[] attributes = type.GetCustomAttributes(true);

                if (attributes.FirstOrDefault(x => x is NodeAttribute) is NodeAttribute nodeAttribute)
                {
                    if (!nodeAttribute.NodeCacherIgnore)
                    {
                        string group = nodeAttribute.Group;

                        if (string.IsNullOrEmpty(group))
                        {
                            group = "ungrouped";
                        }

                        if (!_allBaseNodeGroups.ContainsKey(group))
                        {
                            _allBaseNodeGroups.Add(group, new List<Node>());
                        }

                        Node node = (Node)Activator.CreateInstance(type, type.Name);
                        _allBaseNodeGroups[group].Add(node);
                    }
                }
            }
        }

        public static void CacheBaseNodes()
        {
            if (_nodeInfoList == null)
            {
                _nodeInfoList = new Dictionary<Type, NodeMembersInfo>();

                
                
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
                            CacheBaseNodes(assembly);
                            break;
                    }
                }
            }
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
