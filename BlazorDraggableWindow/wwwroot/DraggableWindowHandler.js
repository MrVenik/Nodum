var DraggableWindowHandler = /** @class */ (function () {
    function DraggableWindowHandler() {
        this.startX = 0;
        this.startY = 0;
        this.isDragging = false;
    }
    DraggableWindowHandler.prototype.StartDragging = function (e) {
        var _this = this;
        e = (e || window.event);
        e.preventDefault();
        if (this.draggableZone == null) {
            this.draggableZone = document.getElementById("draggablezone");
        }
        this.draggableWindow = e.currentTarget.parentElement;
        this.startX = e.clientX;
        this.startY = e.clientY;
        this.isDragging = true;
        if (this.OnStartDragging) {
            this.OnStartDragging(e);
        }
        this.draggableZone.onmousemove = (function (ev) { return _this.DragWindow(ev); });
        this.draggableZone.onmouseup = (function (ev) { return _this.StopDraggingWindow(ev); });
        globalThis.DotNet.invokeMethodAsync("BlazorDraggableWindow", "StartDragWindow", this.draggableWindow.id, e.clientX, e.clientY);
    };
    DraggableWindowHandler.prototype.DragWindow = function (e) {
        if (this.isDragging && this.draggableWindow) {
            e = (e || window.event);
            e.preventDefault();
            var x = (this.draggableWindow.offsetLeft + (e.clientX - this.startX));
            var y = (this.draggableWindow.offsetTop + (e.clientY - this.startY));
            this.draggableWindow.style.left = x + "px";
            this.draggableWindow.style.top = y + "px";
            this.startX = e.clientX;
            this.startY = e.clientY;
            if (this.OnDragging) {
                this.OnDragging(e);
            }
            globalThis.DotNet.invokeMethodAsync("BlazorDraggableWindow", "DragWindow", x, y);
        }
    };
    DraggableWindowHandler.prototype.StopDraggingWindow = function (e) {
        this.isDragging = false;
        this.startX = 0;
        this.startY = 0;
        if (this.OnStopDragging) {
            this.OnStopDragging(e);
        }
        this.draggableZone.onmousemove = null;
        this.draggableZone.onmouseup = null;
        globalThis.DotNet.invokeMethodAsync("BlazorDraggableWindow", "StopDragWindow", e.clientX, e.clientY);
    };
    return DraggableWindowHandler;
}());
var draggableWindowHandler = new DraggableWindowHandler();
//# sourceMappingURL=DraggableWindowHandler.js.map