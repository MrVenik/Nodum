using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace BlazorDraggableWindow
{
    public class ElementOffset
    {
        public double Top { get; set; }
        public double Left { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }

        public static async Task<ElementOffset> GetElementOffset(IJSRuntime jSRuntime, string elementId)
        {
            return new ElementOffset()
            {
                Top = await jSRuntime.InvokeAsync<double>("getElementOffsetTop", elementId),
                Left = await jSRuntime.InvokeAsync<double>("getElementOffsetLeft", elementId),
                Height = await jSRuntime.InvokeAsync<double>("getElementOffsetHeight", elementId),
                Width = await jSRuntime.InvokeAsync<double>("getElementOffsetWidth", elementId)
            };
        }
    }
}
