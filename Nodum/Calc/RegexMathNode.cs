using Nodum.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nodum.Calc
{
    [Serializable]
    [BaseNode(Group = "Calc")]
    public class RegexMathNode : Node
    {
        public override bool IsEditable => false;
        [NodePin(IsInvokeUpdatePins = true, CanSetValue = true)] public string RegexOperation { get; set; }
        [Output] public double Result { get; set; }

        private string _oldRegexOperation;
        private readonly MatchEvaluator _evaluator;

        public RegexMathNode(string name = "Regex Math Node") : base(name)
        {
            _evaluator = new MatchEvaluator(MatchReplacer);
        }

        public override void UpdatePins()
        {
            if (!string.IsNullOrEmpty(RegexOperation) && RegexOperation != _oldRegexOperation)
            {
                string[] matches = Regex.Matches(RegexOperation, @"\b[^\d]\w+").OfType<Match>().Select(m => m.Groups[0].Value).ToArray();

                if (!string.IsNullOrEmpty(_oldRegexOperation))
                {
                    string[] oldMatches = Regex.Matches(_oldRegexOperation, @"\b[^\d]\w+").OfType<Match>().Select(m => m.Groups[0].Value).ToArray();
                    foreach (string match in oldMatches)
                    {
                        if (!matches.Any(m => m == match))
                        {
                            RemoveNodePin(match);
                        }
                    }
                }

                foreach (string match in matches)
                {
                    if (!NodePins.ContainsKey(match))
                    {
                        NodePin inputNodePin = NodePinBuilder.BuildNodePin(match, this, typeof(double), new NodePinOptions() { IsInput = true, IsInvokeUpdate = true });

                        ProtectedTryAddNodePin(inputNodePin);
                    }
                }

                _oldRegexOperation = RegexOperation;
                OnUpdatePins?.Invoke();
            }
        }

        public override void UpdateValue()
        {
            if (!string.IsNullOrEmpty(RegexOperation))
            {
                string math = Regex.Replace(RegexOperation, @"\b[^\d]\w+", _evaluator).Trim();

                Result = Convert.ToDouble(new DataTable().Compute(math, ""));
            }
        }

        private string MatchReplacer(Match match)
        {
            if (NodePins.TryGetValue(match.Value, out NodePin input))
            {
                return input.Value.ToString();
            }
            return string.Empty;
        }
    }


}
