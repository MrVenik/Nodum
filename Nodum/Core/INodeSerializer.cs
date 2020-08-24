using System;
using System.Collections.Generic;
using System.Text;

namespace Nodum.Core
{
    public interface INodeSerializer
    {
        void Serialize(Node node);
        Node Deserialize(string path);
    }
}
