using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Nodum.Core
{
    public class BinnaryNodeSerializer : INodeSerializer
    {
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

        public void Serialize(Node node)
        {
            using (FileStream fs = File.Create($"{node.Name}.dat"))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, node);
            }
        }
    }
}
