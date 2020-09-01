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
                node?.Update();
            }
            return node;
        }

        public Node Deserialize(string nodeName)
        {
            Node node = null;
            try
            {
                using (FileStream fs = File.OpenRead($"{nodeName}.dat"))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    node = (Node)formatter.Deserialize(fs);
                    node?.ReConnectAllPins();
                    node?.ReConnectClones();
                    node?.Update();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
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

        public void SerializeProject(NodumProject project)
        {
            using (FileStream fs = File.Create($"{project.Name}.dat"))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, project);
            }
        }

        public NodumProject DeserializeProject(string nodumProjectName)
        {
            NodumProject nodumProject = null;
            try
            {
                using (FileStream fs = File.OpenRead($"{nodumProjectName}.dat"))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    nodumProject = (NodumProject)formatter.Deserialize(fs);

                    foreach (var group in nodumProject.BaseNodeGroups)
                    {
                        foreach (var node in group.Value)
                        {
                            node?.ReConnectAllPins();
                            node?.ReConnectClones();
                            node?.Update();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return nodumProject;
        }
    }
}
