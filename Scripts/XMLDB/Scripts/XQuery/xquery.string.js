function EncodeHtmlRaw(value) {
    /// <signature>
    ///   <summary>Encodes a string to raw html.</summary>    
    ///   <param name="value" type="string">The string to encode.</param>
    ///   <returns type="string" />
    /// </signature>

    if (!value)
        throw "The \"value\" is required.";

    return $('<div/>').text(value).html();
}

function DecodeHtmlRaw(value) {
    /// <signature>
    ///   <summary>Decodes a raw html to string.</summary>    
    ///   <param name="value" type="string">The raw html to decode.</param>
    ///   <returns type="string" />
    /// </signature>

    if (!value)
        throw "The \"value\" is required.";

    return $('<div/>').html(value).text();
}