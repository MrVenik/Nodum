using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nodum.Core
{
    public class NodumNodeCompiler : INodeCompiler
    {
        public string Name { get; }

        public NodumNodeCompiler(string name)
        {
            Name = name;
        }

        public string BuildNodeClassString(Node node)
        {
            StringBuilder stringBuilder = new StringBuilder();

            AddClassHeader(node, stringBuilder);

            AddClassFields(node, stringBuilder);

            AddClassConstructor(node, stringBuilder);

            AddClassUpdateMethod(node, stringBuilder);

            AddClassGetStrMethod(node, stringBuilder);

            AddClassCloseString(stringBuilder);

            return stringBuilder.ToString();
        }

        private void AddClassCloseString(StringBuilder stringBuilder)
        {
            string closeStr = @"            
    }
}";

            stringBuilder.Append(closeStr);
        }

        private void AddClassGetStrMethod(Node node, StringBuilder stringBuilder)
        {
            string getStrStr = @"
        public override string GetStringForNodePin(NodePin nodePin)
        {
            if (nodePin.Node == this)
            {";

            stringBuilder.Append(getStrStr);



            foreach (var outPin in node.AllOutputNodePins)
            {
                static string MatchReplacer(Match match)
                {
                    return $"{{GetStringForNodePin(NodePins[\"{match.Value}\"])}}";
                }

                MatchEvaluator evaluator = new MatchEvaluator(MatchReplacer);
                string returnStr = Regex.Replace(outPin.Node.GetStringForNodePin(outPin), @"\b[^\d]\w+", evaluator).Trim();

                string outPinStr = @$"
                if (nodePin.Name == ""{outPin.Name}"")
                {{
                    return $""{returnStr}"";
                }}";

                stringBuilder.Append(outPinStr);
            }

            string closeGetStrStr = @"
            }
            return base.GetStringForNodePin(nodePin);
        }";
            stringBuilder.Append(closeGetStrStr);
        }

        private void AddClassUpdateMethod(Node node, StringBuilder stringBuilder)
        {
            string updateStr = @"
        public override void UpdateValue()
        {";
            stringBuilder.Append(updateStr);

            foreach (var outPin in node.AllOutputNodePins)
            {
                string outPinStr = @$"
            {outPin.Name} = {outPin.Node.GetStringForNodePin(outPin)};";

                stringBuilder.Append(outPinStr);
            }

            string closeUpdateStr = @"
        }";
            stringBuilder.Append(closeUpdateStr);
        }

        private void AddClassConstructor(Node node, StringBuilder stringBuilder)
        {
            string constructorStr = @$"
        public {node.Name}Node(string name = ""{node.Name}Node"") : base(name) {{}}";

            stringBuilder.Append(constructorStr);
        }

        private void AddClassFields(Node node, StringBuilder stringBuilder)
        {
            foreach (var pin in node.AllNodePins)
            {
                StringBuilder pinAttrbStrBuilder = new StringBuilder("NodePin(");
                pinAttrbStrBuilder.Append($"IsInput = {pin.IsInput.ToString().ToLower()}, ");
                pinAttrbStrBuilder.Append($"IsOutput = {pin.IsOutput.ToString().ToLower()}, ");
                pinAttrbStrBuilder.Append($"IsOption = {pin.IsOption.ToString().ToLower()}, ");
                pinAttrbStrBuilder.Append($"IsInternalInput = {pin.IsInternalInput.ToString().ToLower()}, ");
                pinAttrbStrBuilder.Append($"IsInternalOutput = {pin.IsInternalOutput.ToString().ToLower()}, ");
                pinAttrbStrBuilder.Append($"IsInvokeUpdate = {pin.IsInvokeUpdate.ToString().ToLower()}, ");
                pinAttrbStrBuilder.Append($"IsInvokeUpdatePins = {pin.IsInvokeUpdatePins.ToString().ToLower()}, ");
                pinAttrbStrBuilder.Append($"CanSetValue = {pin.CanSetValue.ToString().ToLower()}, ");
                pinAttrbStrBuilder.Append($"CanGetValue = {pin.CanGetValue.ToString().ToLower()})");

                string pinStr = @$"
        [{pinAttrbStrBuilder}]
        public {pin.ValueType} {pin.Name} {{ get; set; }}";
                stringBuilder.Append(pinStr);
            }
        }

        private void AddClassHeader(Node node, StringBuilder stringBuilder)
        {
            string usingsStr = @"
using System;
using System.Collections.Generic;
using System.Text;
using Nodum;
using Nodum.Core;
";

            stringBuilder.Append(usingsStr);

            string namespaceStr = @$"
namespace {Name} 
{{";
            stringBuilder.Append(namespaceStr);

            string classStr = @$"
    [Serializable]
    [Node(Group = ""{Name}"")]
    public class {node.Name}Node : Node
    {{";
            stringBuilder.Append(classStr);
        }

        public void Compile(params Node[] nodes)
        {
            throw new NotImplementedException();
        }
    }
}
