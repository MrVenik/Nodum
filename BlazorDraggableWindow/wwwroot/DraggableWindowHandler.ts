interface IMouseEventCallback {
    (event: MouseEvent): void;
}

class DraggableWindowHandler {
    private startX: number;
    private startY: number;

    private isDragging: boolean;

    private draggableWindow: HTMLElement = undefined;

    private dotNetObject = undefined;

    public constructor() {
        this.startX = 0;
        this.startY = 0;
        this.isDragging = false;
    }

    public Initialize(obj) {
        this.dotNetObject = obj;
    }

    public PositionWindow(windowId: string, x: number, y: number) {
        var window: HTMLElement = document.getElementById(windowId);

        if (window) {
            window.style.left = x + "px";
            window.style.top = y + "px";
        }
    }

    public StartDragging(e: MouseEvent) {
        if (this.dotNetObject) {
            e = (e || window.event) as MouseEvent;
            e.preventDefault();

            this.draggableWindow = (e.currentTarget as HTMLElement).parentElement;

            this.startX = e.clientX;
            this.startY = e.clientY;

            this.isDragging = true;

            document.onmousemove = ((ev) => this.DragWindow(ev));
            document.onmouseup = ((ev) => this.StopDraggingWindow(ev));

            this.dotNetObject.invokeMethodAsync("StartDragWindow", this.draggableWindow.id);
        }
    }

    public DragWindow(e: MouseEvent) {
        if (this.dotNetObject && this.isDragging && this.draggableWindow) {
            e = (e || window.event) as MouseEvent;
            e.preventDefault();

            var x: number = (this.draggableWindow.offsetLeft + (e.clientX - this.startX))
            var y: number = (this.draggableWindow.offsetTop + (e.clientY - this.startY))

            this.draggableWindow.style.left = x + "px";
            this.draggableWindow.style.top = y + "px";

            this.startX = e.clientX;
            this.startY = e.clientY;

            this.dotNetObject.invokeMethodAsync("DragWindow", x, y);
        }

    }

    public StopDraggingWindow(e: MouseEvent) {
        if (this.dotNetObject) {

            this.isDragging = false;
            this.startX = 0;
            this.startY = 0;

            document.onmousemove = null;
            document.onmouseup = null;

            this.dotNetObject.invokeMethodAsync("StopDragWindow");
        }
    }
}

var draggableWindowHandler: DraggableWindowHandler = new DraggableWindowHandler();