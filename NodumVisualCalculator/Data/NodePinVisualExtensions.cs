using Nodum.Core;

namespace NodumVisualCalculator.Data
{
    public static class NodePinVisualExtensions
    {
        public static string GetInputElementId(this NodePin nodePin) => $"{nodePin.Name}_{nodePin.Guid}_Input";
        public static string GetOutputElementId(this NodePin nodePin) => $"{nodePin.Name}_{nodePin.Guid}_Output";
    }
}
