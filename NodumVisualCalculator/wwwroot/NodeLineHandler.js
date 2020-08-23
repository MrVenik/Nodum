var Line = /** @class */ (function () {
    function Line() {
    }
    return Line;
}());
var NodeLineHandler = /** @class */ (function () {
    function NodeLineHandler() {
        draggableWindowHandler.OnDragging = this.UpdateLines;
    }
    NodeLineHandler.prototype.UpdateLines = function () {
        var lineElements = document.getElementsByClassName("node-line");
        for (var i = 0; i < lineElements.length; i++) {
            var lineElement = lineElements[i];
            var lineElementId = lineElement.id;
            var lineElementPinIds = lineElementId.split("+");
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
    };
    NodeLineHandler.GetPath = function (fromX, fromY, toX, toY) {
        var x0, y0, x1, y1, x2, y2, x3, y3;
        x0 = fromX;
        y0 = fromY;
        x3 = toX;
        y3 = toY;
        x1 = x0 + ((x3 - x0) / 2.0);
        y1 = y0;
        x2 = x0 + ((x1 - x0) / 2.0);
        y2 = y3;
        return "M " + x0 + ", " + y0 + " C " + x1 + ", " + y1 + " " + x2 + ", " + y2 + " " + x3 + ", " + y3;
    };
    return NodeLineHandler;
}());
var nodeLineHandler = new NodeLineHandler();
//# sourceMappingURL=NodeLineHandler.js.map