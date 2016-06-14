(function ($) {
    $.fn.hasVerticalScrollBar = function () {
        /// <signature>
        ///   <summary>Detect if the element have vertical scrollbar visible.</summary>  
        ///   <returns type="bool" />
        /// </signature> 
        return this.get(0).scrollHeight > this.outerHeight();
    }
})(jQuery);

(function ($) {
    $.fn.hasHorizontalScrollBar = function () {
        /// <signature>
        ///   <summary>Detect if the element have horizontal scrollbar visible.</summary>    
        ///   <returns type="bool" />
        /// </signature> 
        return this.get(0).scrollWidth > this.outerWidth();
    }
})(jQuery);


function addSeparatorsNF(nStr, inD, outD, sep) {
    nStr += '';
    var dpos = nStr.indexOf(inD);
    var nStrEnd = '';
    if (dpos != -1) {
        nStrEnd = outD + nStr.substring(dpos + 1, nStr.length);
        nStr = nStr.substring(0, dpos);
    }
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(nStr)) {
        nStr = nStr.replace(rgx, '$1' + sep + '$2');
    }
    return nStr + nStrEnd;
}