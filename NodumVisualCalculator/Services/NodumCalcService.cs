using Nodum.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NodumVisualCalculator.Services
{
    public class NodumCalcService
    {
        private Dictionary<string, List<Node>> _nodeGroups;
        public IReadOnlyDictionary<string, List<Node>> NodeGroups => _nodeGroups;

        private INodeSerializer _nodeSerializer;

        public NodumCalcService(INodeSerializer nodeSerializer)
        {
            _nodeGroups = new Dictionary<string, List<Node>>();
            _nodeSerializer = nodeSerializer;
            GetNodeGropus();
        }

        private void GetNodeGropus()
        {
            if (_nodeSerializer != null)
            {
                NodeCacher.CacheBaseNodes();

                foreach (var group in NodeCacher.AllBaseNodeGroups)
                {
                    if (!_nodeGroups.ContainsKey(group.Key))
                    {
                        _nodeGroups.Add(group.Key, group.Value);
                    }
                }
            }
        }

        public void CreateNewGroup(string groupName)
        {
            if (!string.IsNullOrEmpty(groupName) && !_nodeGroups.ContainsKey(groupName))
            {
                _nodeGroups.Add(groupName, new List<Node>());
            }
        }

        public void AddNode(Node node, string groupName = "ungrouped")
        {
            if (string.IsNullOrEmpty(groupName))
            {
                groupName = "ungrouped";
            }
            if (_nodeGroups.ContainsKey(groupName))
            {
                _nodeGroups[groupName].Add(node);
            }
        }

        public void DeleteNode(Node node, string groupName = "ungrouped")
        {
            if (string.IsNullOrEmpty(groupName))
            {
                groupName = "ungrouped";
            }
            if (_nodeGroups.ContainsKey(groupName))
            {
                _nodeGroups[groupName].Remove(node);
            }
        }
    }
}
