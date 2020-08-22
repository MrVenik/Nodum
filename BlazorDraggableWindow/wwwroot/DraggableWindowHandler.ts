interface IMouseEventCallback {
    (event: MouseEvent): void;
}

class DraggableWindowHandler {
    private startX: number;
    private startY: number;

    private isDragging: boolean;

    private draggableZone: HTMLElement;
    private draggableWindow: HTMLElement;

    public OnStartDragging: IMouseEventCallback;
    public OnDragging: IMouseEventCallback;
    public OnStopDragging: IMouseEventCallback;

    public constructor() {
        this.startX = 0;
        this.startY = 0;
        this.isDragging = false;
    }

    public StartDragging(e: MouseEvent) {
        e = (e || window.event) as MouseEvent;
        e.preventDefault();

        if (this.draggableZone == null) {
            this.draggableZone = document.getElementById("draggablezone");
        }

        this.draggableWindow = (e.currentTarget as HTMLElement).parentElement;

        this.startX = e.clientX;
        this.startY = e.clientY;

        this.isDragging = true;

        if (this.OnStartDragging) {
            this.OnStartDragging(e);
        }

        this.draggableZone.onmousemove = ((ev) => this.DragWindow(ev));
        this.draggableZone.onmouseup = ((ev) => this.StopDraggingWindow(ev));

        globalThis.DotNet.invokeMethodAsync("BlazorDraggableWindow", "StartDragWindow", this.draggableWindow.id, e.clientX, e.clientY);
    }

    public DragWindow(e: MouseEvent) {
        if (this.isDragging && this.draggableWindow) {
            e = (e || window.event) as MouseEvent;
            e.preventDefault();

            var x: number = (this.draggableWindow.offsetLeft + (e.clientX - this.startX))
            var y: number = (this.draggableWindow.offsetTop + (e.clientY - this.startY))

            this.draggableWindow.style.left = x + "px";
            this.draggableWindow.style.top = y + "px";

            this.startX = e.clientX;
            this.startY = e.clientY;

            if (this.OnDragging) {
                this.OnDragging(e);
            }

            globalThis.DotNet.invokeMethodAsync("BlazorDraggableWindow", "DragWindow", x, y);
        }

    }

    public StopDraggingWindow(e: MouseEvent) {
        this.isDragging = false;
        this.startX = 0;
        this.startY = 0;

        if (this.OnStopDragging) {
            this.OnStopDragging(e);
        }

        this.draggableZone.onmousemove = null;
        this.draggableZone.onmouseup = null;

        globalThis.DotNet.invokeMethodAsync("BlazorDraggableWindow", "StopDragWindow", e.clientX, e.clientY);
    }
}

var draggableWindowHandler: DraggableWindowHandler = new DraggableWindowHandler();