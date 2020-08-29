using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nodum.Core
{
    [Serializable]
    public class NodumProject
    {
        public string Name { get; set; }
        public MainNode MainNode { get; private set; }

        private Dictionary<string, List<Node>> _baseNodeGroups;
        private Dictionary<string, List<Node>> _nodeGroups;

        public NodumProject(string name)
        {
            _baseNodeGroups = new Dictionary<string, List<Node>>();
            _nodeGroups = new Dictionary<string, List<Node>>();

            Name = name;

            MainNode = new MainNode(name);
        }

        public IReadOnlyDictionary<string, List<Node>> BaseNodeGroups => _baseNodeGroups;
        public IReadOnlyDictionary<string, List<Node>> NodeGroups => _nodeGroups;

        public void GetBaseNodeGroups()
        {
            NodeCacher.CacheBaseNodes();

            foreach (var group in NodeCacher.AllBaseNodeGroups)
            {
                AddBaseGroup(group.Key, group.Value);
            }
        }

        private void CreateNewBaseGroup(string groupName)
        {
            if (!string.IsNullOrEmpty(groupName) && !_baseNodeGroups.ContainsKey(groupName))
            {
                _baseNodeGroups.Add(groupName, new List<Node>());
            }
        }

        public void AddBaseGroup(string groupName, List<Node> nodes)
        {
            CreateNewBaseGroup(groupName);
            _baseNodeGroups[groupName] = nodes;
        }

        private void CreateNewGroup(string groupName)
        {
            if (!string.IsNullOrEmpty(groupName) && !_nodeGroups.ContainsKey(groupName))
            {
                _nodeGroups.Add(groupName, new List<Node>());
            }
        }

        public void AddGroup(string groupName, List<Node> nodes)
        {
            CreateNewGroup(groupName);
            _nodeGroups[groupName] = nodes;
        }

        public void AddNode(Node node, string groupName = "ungrouped")
        {
            if (string.IsNullOrEmpty(groupName))
            {
                groupName = "ungrouped";
            }
            if (!_nodeGroups.ContainsKey(groupName))
            {
                CreateNewGroup(groupName);
            }
            _nodeGroups[groupName].Add(node);
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

        [OnDeserialized]
        public virtual void OnDeserialized(StreamingContext context)
        {
            if (_baseNodeGroups == null)
            {
                _baseNodeGroups = new Dictionary<string, List<Node>>();
            }
            if (_nodeGroups == null)
            {
                _nodeGroups = new Dictionary<string, List<Node>>();
            }
        }
    }
}