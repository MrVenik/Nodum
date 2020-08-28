using System;
using System.Collections.Generic;
using System.Text;

namespace Nodum.Core
{
    public interface INodeSerializer
    {
        void Serialize(Node node);
        void SerializeGroup(string groupName, params Node[] nodes);
        void SerializeAllGroups(Dictionary<string, List<Node>> nodeGroups);
        Node Deserialize(string nodePath);
        List<Node> DeserializeGroup(string groupPath);
        Dictionary<string, List<Node>> DeserializeAllGroups(string allGroupsPath);
    }
}
