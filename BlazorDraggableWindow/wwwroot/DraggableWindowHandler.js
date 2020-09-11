var DraggableWindowHandler = /** @class */ (function () {
    function DraggableWindowHandler() {
        this.draggableWindow = undefined;
        this.dotNetObject = undefined;
        this.startX = 0;
        this.startY = 0;
        this.isDragging = false;
    }
    DraggableWindowHandler.prototype.Initialize = function (obj) {
        this.dotNetObject = obj;
    };
    DraggableWindowHandler.prototype.PositionWindow = function (windowId, x, y) {
        var window = document.getElementById(windowId);
        if (window) {
            window.style.left = x + "px";
            window.style.top = y + "px";
        }
    };
    DraggableWindowHandler.prototype.StartDragging = function (e) {
        var _this = this;
        if (this.dotNetObject) {
            e = (e || window.event);
            e.preventDefault();
            this.draggableWindow = e.currentTarget.parentElement;
            this.startX = e.clientX;
            this.startY = e.clientY;
            this.isDragging = true;
            document.onmousemove = (function (ev) { return _this.DragWindow(ev); });
            document.onmouseup = (function (ev) { return _this.StopDraggingWindow(ev); });
            this.dotNetObject.invokeMethodAsync("StartDragWindow", this.draggableWindow.id, e.clientX, e.clientY);
        }
    };
    DraggableWindowHandler.prototype.DragWindow = function (e) {
        if (this.dotNetObject && this.isDragging && this.draggableWindow) {
            e = (e || window.event);
            e.preventDefault();
            var x = (this.draggableWindow.offsetLeft + (e.clientX - this.startX));
            var y = (this.draggableWindow.offsetTop + (e.clientY - this.startY));
            this.draggableWindow.style.left = x + "px";
            this.draggableWindow.style.top = y + "px";
            this.startX = e.clientX;
            this.startY = e.clientY;
            this.dotNetObject.invokeMethodAsync("DragWindow", x, y);
        }
    };
    DraggableWindowHandler.prototype.StopDraggingWindow = function (e) {
        if (this.dotNetObject) {
            this.isDragging = false;
            this.startX = 0;
            this.startY = 0;
            document.onmousemove = null;
            document.onmouseup = null;
            this.dotNetObject.invokeMethodAsync("StopDragWindow", e.clientX, e.clientY);
        }
    };
    return DraggableWindowHandler;
}());
var draggableWindowHandler = new DraggableWindowHandler();
//# sourceMappingURL=DraggableWindowHandler.js.map