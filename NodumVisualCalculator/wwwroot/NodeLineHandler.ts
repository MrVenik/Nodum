interface IMouseEventCallback {
    (event: MouseEvent): void;
}

interface IDraggableWindowHandler {
    OnStartDragging: IMouseEventCallback;
    OnDragging: IMouseEventCallback;
    OnStopDragging: IMouseEventCallback;
    StartDragging(e: MouseEvent): void;
    DragWindow(e: MouseEvent): void;
    StopDraggingWindow(e: MouseEvent): void;
}

declare var draggableWindowHandler: IDraggableWindowHandler;

class Line {
    ID: string;
    FromId: string;
    ToId: string;
}

class NodeLineHandler {
    public constructor() {
        //draggableWindowHandler.OnDragging = this.UpdateLines;
    }

    public UpdateLines() {
        var lineElements = document.getElementsByClassName("node-line");
        for (var i = 0; i < lineElements.length; i++) {
            var lineElement: HTMLElement = lineElements[i] as HTMLElement;
            var lineElementId: string = lineElement.id;
            var lineElementPinIds = lineElementId.split("+")
            if (lineElementPinIds.length == 2) {
                var fromElement = document.getElementById(lineElementPinIds[0]);
                var toElement = document.getElementById(lineElementPinIds[1]);
                if (fromElement && toElement) {
                    var fromX = fromElement.getBoundingClientRect().left + pageXOffset + 12;
                    var fromY = fromElement.getBoundingClientRect().top + pageYOffset + 12;

                    var toX = toElement.getBoundingClientRect().left + pageXOffset + 12;
                    var toY = toElement.getBoundingClientRect().top + pageYOffset + 12;

                    var path = NodeLineHandler.GetPath(fromX, fromY, toX, toY);

                    lineElement.setAttribute("d", path);
                }
            }
        }
    }

    private static GetPath(fromX: number, fromY: number, toX: number, toY: number): string {

        var x0: number, y0: number, x1: number, y1: number, x2: number, y2: number, x3: number, y3: number;

        x0 = fromX;
        y0 = fromY;

        x3 = toX;
        y3 = toY;

        x1 = x0 + ((x3 - x0) / 2.0);
        y1 = y0;

        x2 = x0 + ((x1 - x0) / 2.0);
        y2 = y3;

        return `M ${x0}, ${y0} C ${x1}, ${y1} ${x2}, ${y2} ${x3}, ${y3}`;
    }
}

var nodeLineHandler: NodeLineHandler = new NodeLineHandler();