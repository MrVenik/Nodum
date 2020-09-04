using Nodum.Calc;
using Nodum.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Stopwatch stopWatch = Stopwatch.StartNew();
            _nodeSerializer = nodeSerializer;

            MainProject = _nodeSerializer.DeserializeProject("CalcProject");

            if (MainProject == null)
            {
                MainProject = new NodumProject("CalcProject");
            }

            MainProject.GetBaseNodeGroups();
            stopWatch.Stop();
            Console.WriteLine($"NodumCalcService: Initialization: {stopWatch.ElapsedMilliseconds}ms");
        }

        public void Save()
        {
            Stopwatch stopWatch = Stopwatch.StartNew();
            _nodeSerializer.SerializeProject(MainProject);
            stopWatch.Stop();
            Console.WriteLine($"NodumCalcService: Serialization: {stopWatch.ElapsedMilliseconds}ms");
        }
    }
}
