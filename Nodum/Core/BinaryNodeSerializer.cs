using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Nodum.Core
{
    public class BinaryNodeSerializer : INodeSerializer
    {
        public byte[] SerializeToByteArray(Node node)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, node);
                return ms.ToArray();
            }
        }

        public Node DeserializeFromByteArray(byte[] bytes)
        {
            Node node = null;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                node = (Node)formatter.Deserialize(ms);
                node?.ReConnectAllPins();
            }
            return node;
        }

        public Node Deserialize(string path)
        {
            Node node = null;
            using (FileStream fs = File.OpenRead($"{path}.dat"))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                node = (Node)formatter.Deserialize(fs);
                node?.ReConnectAllPins();
            }
            return node;
        }

        public Dictionary<string, List<Node>> DeserializeAllGroups(string groupFolderPath)
        {
            throw new NotImplementedException();
        }

        public List<Node> DeserializeGroup(string groupPath)
        {
            throw new NotImplementedException();
        }

        public void Serialize(Node node)
        {
            using (FileStream fs = File.Create($"{node.Name}.dat"))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, node);
            }
        }

        public void SerializeAllGroups(Dictionary<string, List<Node>> nodeGroups)
        {
            throw new NotImplementedException();
        }

        public void SerializeGroup(string groupName, params Node[] nodes)
        {
            throw new NotImplementedException();
        }
    }
}
