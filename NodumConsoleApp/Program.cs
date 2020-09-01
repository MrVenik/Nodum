using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Nodum.Core;
using Nodum.Calc;
using Nodum.Reflection;
using System.Reflection;

namespace NodumConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Node> nodes = new List<Node>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Type type = typeof(Math);
            
            nodes.AddRange(CreateMethodNodesForType(type, Console.Out));

            foreach (var node in nodes)
            {
                Debug(node);
            }
        }

        private static List<Node> CreateMethodNodesForType(Type type, TextWriter errorStrem)
        {
            List<Node> nodes = new List<Node>();
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (var method in methods)
            {
                try
                {
                    nodes.Add(new MethodNode(method, null));
                }
                catch (Exception ex)
                {
                    errorStrem.WriteLine(ex.Message);
                }
            }
            return nodes;
        }

        private static void Debug(Node node, bool withGuid = false)
        {
            string nodeStr = GetNodeString(node, withGuid);
            Console.WriteLine(nodeStr);
        }

        private static string GetNodeString(Node node, bool withGuid = false)
        {
            string nodeStr = $"\n{node.GetType()} {node.Name} {{\n";
            foreach (var pin in node.AllNodePins)
            {
                if (withGuid)
                {
                    nodeStr += $"\t{pin.ValueType} {pin.Name}_{pin.Guid} = {pin.Value};\n";
                }
                else
                {
                    nodeStr += $"\t{pin.ValueType} {pin.Name} = {pin.Value};\n";
                }
            }
            foreach (var item in node.InternalNodes)
            {
                nodeStr += GetNodeString(item, withGuid);
            }
            nodeStr += "}\n";
            return nodeStr;
        }
    }
}
