using Nodum.Calc;
using Nodum.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace NodumVisualCalculator.Services
{
    public class NodumCalcService
    {
        public NodumProject MainProject { get; private set; }

        private INodeSerializer _nodeSerializer;

        public NodumCalcService(INodeSerializer nodeSerializer)
        {
            _nodeSerializer = nodeSerializer;

            MainProject = _nodeSerializer.DeserializeProject("CalcProject");

            if (MainProject == null)
            {
                MainProject = new NodumProject("CalcProject");
            }

            MainProject.GetBaseNodeGroups();
        }

        public void Save()
        {
            _nodeSerializer.SerializeProject(MainProject);
        }
    }
}
