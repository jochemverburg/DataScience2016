function LockerHelper(parameters) {
    /// <signature>
    ///   <summary>Initialize a DialogHelper.
    ///   &#10;
    ///   &#10;Parameters Keys:    
    ///   &#10;1. JQuery container: The container to use as base to lock/unlock.
    ///   &#10;2. string text: The text to show while the container is locked.
    ///   </summary>    
    ///   <param name="parameters" type="PlainObject">A set of key/value pairs that configure the locker.</param>    
    /// </signature>    

    var self = this;
    var container = parameters.container || null;
    var text = parameters.text || "Locked.";

    this.LockScreen = function () {
        /// <signature>
        ///   <summary>Lock the container.
        ///   </summary>            
        /// </signature>    

        container = "html";

        var centerStyle = "top: 50%; bottom: 0; margin-top: auto; margin-bottom: auto; left: 0; right: 0; margin-left: auto; margin-right: auto; px; text-align: center;";
        var subContainerStyle = "left: 0; right: 0; top: 0; bottom: 0; z-index: 9999;";        

        if ($(container).html() == "") {
            centerStyle += "position: relative;";
            subContainerStyle += "position: relative;";
        }
        else {
            centerStyle += "position: absolute;";
            subContainerStyle += "position: absolute;";
        }

        $(container).css("position", "relative");        
        $(container).append("<div id=\"LockerHelper_LockDiv\" style=\"" + subContainerStyle + " color: #ffffff; background: rgba(0, 0, 0, 0.8);\" ><div style=\"" + centerStyle + "\"><span>" + text + "</span><br /><progress></progress></div></div>");
    }

    this.Lock = function () {
        /// <signature>
        ///   <summary>Disable the screen.
        ///   </summary>            
        /// </signature>  
        $(container).find("*").each(function (index, element) {
            $(element).attr("disabled", "disabled");
        });
    }

    this.UnlockScreen = function () {
        /// <signature>
        ///   <summary>Unlock the screen.
        ///   </summary>            
        /// </signature>    
        container = "html";
        $(container).children("#LockerHelper_LockDiv").remove();
    }

    this.Unlock = function () {
        /// <signature>
        ///   <summary>Enable the container elements.
        ///   </summary>            
        /// </signature>  

        $(container).find("*").each(function (index, element) {
            $(element).removeAttr("disabled");
        });
    }
}