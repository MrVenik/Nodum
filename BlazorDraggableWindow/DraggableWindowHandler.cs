using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorDraggableWindow
{
    public class DraggableWindowHandler : IDisposable
    {
        private IJSRuntime _jsRuntime;
        private DotNetObjectReference<DraggableWindowHandler> _dotnetObject;


        private readonly Dictionary<string, DraggableWindow> _draggableWindows;
        public IReadOnlyDictionary<string, DraggableWindow> DraggableWindows => _draggableWindows;

        private DraggableWindow _selectedWindow;

        public DraggableWindowHandler(IJSRuntime jsRuntime)
        {
            Console.WriteLine("Creating!");
            _dotnetObject = DotNetObjectReference.Create(this);
            _jsRuntime = jsRuntime;
            _draggableWindows = new Dictionary<string, DraggableWindow>();
        }

        public async Task Initialize()
        {
            Console.WriteLine("Initializing!");
            await _jsRuntime.InvokeVoidAsync("draggableWindowHandler.Initialize", _dotnetObject);
        }

        public void AddDraggableWindow(DraggableWindow draggableWindow)
        {
            if (!_draggableWindows.ContainsKey(draggableWindow.WindowId))
            {
                _draggableWindows.Add(draggableWindow.WindowId, draggableWindow);
            }
        }

        public void RemoveDraggableWindow(DraggableWindow draggableWindow)
        {
            _draggableWindows.Remove(draggableWindow.WindowId);
        }

        [JSInvokable]
        public void StartDragWindow(string windowId, double x, double y)
        {
            if (_draggableWindows.TryGetValue(windowId, out DraggableWindow window))
            {
                _selectedWindow = window;

                _selectedWindow.PositionX = x;
                _selectedWindow.PositionY = y;

                _selectedWindow.OnStartDragingWindow?.Invoke(_selectedWindow);
            }
        }

        [JSInvokable]
        public void DragWindow(double x, double y)
        {
            if (_selectedWindow != null)
            {
                _selectedWindow.PositionX = x;
                _selectedWindow.PositionY = y;

                _selectedWindow?.OnDragingWindow?.Invoke(_selectedWindow);
            }
        }

        [JSInvokable]
        public void StopDragWindow(double x, double y)
        {
            if (_selectedWindow != null)
            {
                _selectedWindow.PositionX = x;
                _selectedWindow.PositionY = y;

                _selectedWindow.OnStopDragingWindow?.Invoke(_selectedWindow);
                _selectedWindow = null;
            }
        }

        public void Dispose()
        {
            Console.WriteLine("Disposing!");
            _dotnetObject.Dispose();
        }
    }
}
