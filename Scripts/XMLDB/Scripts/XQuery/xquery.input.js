function InputHelper() {
    var self = this;
    //Constants
    this.INPUTS = {
        KEY_UP: 0,
        KEY_DOWN: 1,
        KEY_LEFT: 2,
        KEY_RIGHT: 3,
        KEY_SPACE: 4,
        MOUSE_LEFT: 5,
        MOUSE_MIDDLE: 6,
        MOUSE_RIGHT: 7,
        KEY_LEFT_CONTROL: 8
    }

    //Private Variables
    var keys = new Array();
    var mousePos = {
        x: 0,
        y: 0
    };

    //Initialization
    for (var key = 0; key < keys.length; key++) {
        keys[key] = false;
    }

    document.onkeydown = function (event) {
        var keyCode;

        if (event == null) {
            keyCode = window.event.keyCode;
        }
        else {
            keyCode = event.keyCode;
        }

        switch (keyCode) {
            case 37:
                keys[self.INPUTS.KEY_LEFT] = true;
                break;
            case 38:
                keys[self.INPUTS.KEY_UP] = true;
                break;
            case 39:
                keys[self.INPUTS.KEY_RIGHT] = true;
                break;
            case 40:
                keys[self.INPUTS.KEY_DOWN] = true;
                break;
            case 32:
                keys[self.INPUTS.KEY_SPACE] = true;
                break;
            case 17:
                keys[self.INPUTS.KEY_LEFT_CONTROL] = true;
                break;
        }
    }

    document.onkeyup = function (event) {
        var keyCode;

        if (event == null) {
            keyCode = window.event.keyCode;
        }
        else {
            keyCode = event.keyCode;
        }

        switch (keyCode) {
            case 37:
                keys[self.INPUTS.KEY_LEFT] = false;
                break;
            case 38:
                keys[self.INPUTS.KEY_UP] = false;
                break;
            case 39:
                keys[self.INPUTS.KEY_RIGHT] = false;
                break;
            case 40:
                keys[self.INPUTS.KEY_DOWN] = false;
                break;
            case 32:
                keys[self.INPUTS.KEY_SPACE] = false;
                break;
            case 17:
                keys[self.INPUTS.KEY_LEFT_CONTROL] = false;
                break;
        }
    }

    document.onmousemove = function (event) {
        if (event.pageX || event.pageY) {
            mousePos.x = event.pageX;
            mousePos.y = event.pageY;
        }
        else if (event.layerX || event.layerX) {
            mousePos.x = event.layerX;
            mousePos.y = event.layerY;
        } else if (event.offsetX || event.offsetX) {
            mousePos.x = event.offsetX;
            mousePos.y = event.offsetY;
        }
        else {
            if (e) {
                mousePos.x = e.clientX + document.body.scrollLeft + document.documentElement.scrollLeft;
                mousePos.y = e.clientY + document.body.scrollTop + document.documentElement.scrollTop;
            }
        }
    }

    document.onmousedown = function (event) {
        if (event.button == 0)
            keys[self.INPUTS.MOUSE_LEFT] = true;
        else if (event.button == 1)
            keys[self.INPUTS.MOUSE_MIDDLE] = true;
        else if (event.button == 2)
            keys[self.INPUTS.MOUSE_RIGHT] = true;
    }

    document.onmouseup = function (event) {
        if (event.button == 0)
            keys[self.INPUTS.MOUSE_LEFT] = false;
        else if (event.button == 1)
            keys[self.INPUTS.MOUSE_MIDDLE] = false;
        else if (event.button == 2)
            keys[self.INPUTS.MOUSE_RIGHT] = false;
    }

    //Public Methods
    this.IsPressed = function (key) {
        return keys[key];
    }

    this.IsReleased = function (key) {
        return !keys[key];
    }

    this.MouseX = function () {
        return mousePos.x;
    }

    this.MouseY = function () {
        return mousePos.y;
    }
}