using System;
using System.Collections.Generic;
using System.Text;

namespace Nodum.Core
{
    public interface INodeSerializer
    {
        void Serialize(Node node);
        Node Deserialize(string nodeName);

        void SerializeProject(NodumProject project);
        NodumProject DeserializeProject(string nodumProjectName);
    }
}
