function getElementOffsetTop(elmntId) {
    var elmnt = document.getElementById(elmntId);

    if (elmnt === null) {
        return 0;
    }

    var box = elmnt.getBoundingClientRect();

    return box.top + pageYOffset;
}

function getElementOffsetLeft(elmntId) {
    var elmnt = document.getElementById(elmntId);

    if (elmnt === null) {
        return 0;
    }

    var box = elmnt.getBoundingClientRect(); 

    return box.left + pageXOffset;
}
