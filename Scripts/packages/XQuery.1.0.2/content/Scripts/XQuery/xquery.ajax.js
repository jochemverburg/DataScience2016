function AjaxHelper(parameters) {
    /// <signature>
    ///   <summary>Initialize an AjaxHelper.
    ///   &#10;
    ///   &#10;Parameters Keys:    
    ///   &#10;1. string url: The url to make the request.
    ///   &#10;2. [PlainObject data]: The data to send with the request.
    ///   &#10;3. [JQuery sender]: The JQuery Object that are the sender of the request. It's used to change it's own text.
    ///   &#10;4. [JQuery container]: The JQuery Object that are the container of the request. It's also cab be used to lock and unlock while the request is running when locker isn't specified.
    ///   &#10;5. [string text]: The text to show on sender while ajax is running.
    ///   &#10;6. [Function beforeSend]: A callback function to run before the request.
    ///   &#10;7. [Function success]: A callback function to run when the request succeeds.
    ///   &#10;8. [Function error]: A callback function to run when the request not succeeds.
    ///   &#10;9. [Function complete]: A callback function to run when the request ends.
    ///   &#10;10. [Boolean traditional]: Set the data type to traditional mode.
    ///   &#10;11. [JQuery locker]: It's used to lock and unlock while the request is running.
    ///   </summary>    
    ///   <param name="parameters" type="PlainObject">A set of key/value pairs that configure the ajax request.</param>    
    /// </signature>    
	var self = this;

	if (parameters.beforeSend && !(parameters.beforeSend instanceof Function))
	    throw "The \"beforeSend\" parameter isn't an instance of Function.";

	if (parameters.success && !(parameters.success instanceof Function))
	    throw "The \"success\" parameter isn't an instance of Function.";

	if (parameters.error && !(parameters.error instanceof Function))
	    throw "The \"error\" parameter isn't an instance of Function.";

	if (parameters.complete && !(parameters.complete instanceof Function))
	    throw "The \"complete\" parameter isn't an instance of Function.";

	var url = parameters.url || null;
	var data = parameters.data || null;
	var sender = parameters.sender || null;
	var container = parameters.container || null;
	var locker = parameters.locker || container;
	var text = parameters.text || "Loading...";
	var beforeSend = parameters.beforeSend || function () { };
	var success = parameters.success || function (data) { };
	var error = parameters.error || function (exception) { };
	var complete = parameters.complete || function () { };
	var traditional = parameters.traditional || null;
	var async = parameters.async;

	if (sender) {
	    var form = $(sender).parents('form');
	    if (form) {

	        if (!url)
	            url = form.attr("action");

	        if (!data)
	            data = form.serialize();

	        if (!container)
	            container = form;

	        if (!locker)
	            locker = container;
	    }
	}
        
	if (!url)
	    throw "The \"url\" is required.";

	this.Post = function () {	
	    /// <signature>
	    ///   <summary>Send a POST to the url defined.</summary>  
        ///   <returns type="Boolean" />
	    /// </signature> 

		var prevText = null;

		if (sender)
			prevText = $(sender).text();

		var _locker = new LockerHelper({
		    container: locker,
		    text: text
		});

		$.ajax({
			url: url,
			data: data,
			type: "post",
			async: async,
            traditional: traditional,
			beforeSend: function () {
			    if (sender) {
			        $(sender).text(text);
			        $(sender).attr("disabled", "disabled");
			    }

			    _locker.Lock();

				beforeSend();
			},
			success: function (data) {
				success(data);
			},
			error: function (jqXHR, textStatus, errorThrown) {

			    var str = jqXHR.responseText;
			    var n = str.search("<title>") + 1;
			    var e = str.search("</title>");
			    str = str.substring(n, e);

			    var errorMessage = str.substring(str.search(">") + 1, e);
			    if (!errorMessage)
			        error(errorThrown);
			    else
			        error(errorMessage);
			},
			complete: function () {
			    if (sender) {
			        $(sender).text(prevText);
			        $(sender).removeAttr("disabled");
			    }

			    _locker.Unlock();

			    complete();
			}
		});

		return false;
	}

	this.Get = function () {
	    /// <signature>
	    ///   <summary>Send a GET to the url defined.</summary>  
	    ///   <returns type="Boolean" />
	    /// </signature> 

	    var prevText = null;

	    if (sender)
	        prevText = $(sender).text();

	    var _locker = new LockerHelper({
	        container: locker,
	        text: text
	    });

	    $.ajax({
	        url: url,
	        data: data,
	        type: "get",
	        async: async,
	        traditional: traditional,
	        beforeSend: function () {
	            if (sender) {
	                $(sender).text(text);
	                $(sender).attr("disabled", "disabled");
	            }

	            _locker.Lock();

	            beforeSend();
	        },
	        success: function (data) {
	            success(data);
	        },
	        error: function (jqXHR, textStatus, errorThrown) {

	            var str = jqXHR.responseText;
	            var n = str.search("<title>") + 1;
	            var e = str.search("</title>");
	            str = str.substring(n, e);

	            var errorMessage = str.substring(str.search(">") + 1, e);
	            if (!errorMessage)
	                error(errorThrown);
	            else
	                error(errorMessage);
	        },
	        complete: function () {
	            if (sender) {
	                $(sender).text(prevText);
	                $(sender).removeAttr("disabled");
	            }

	            _locker.Unlock();

	            complete();
	        }
	    });

	    return false;
	}

	this.Load = function () {
	    /// <signature>
	    ///   <summary>Load a html inside the container.</summary>
	    /// </signature> 	           

	    if (!container)
	        throw "The \"container\" is required.";

		var prevText = null;

	    if (sender)
	        prevText = $(sender).text();
			
	    var _locker = new LockerHelper({
	        container: locker,
	        text: text
	    });

	    $.ajax({
	        url: url,
	        data: data,
	        async: async,
	        traditional: traditional,            
	        type: "get",
	        beforeSend: function () {
	            if (sender) {
	                $(sender).text(text);
	                $(sender).attr("disabled", "disabled");
	            }
	            
	            _locker.Lock();

	            beforeSend();
	        },
	        success: function (data) {	            
	            $(container).html(data);
	            success(data);
	        },
	        error: function (jqXHR, textStatus, errorThrown) {

	            var str = jqXHR.responseText;
	            var n = str.search("<title>") + 1;
	            var e = str.search("</title>");
	            str = str.substring(n, e);

	            var errorMessage = str.substring(str.search(">") + 1, e);
	            if (!errorMessage)
	                error(errorThrown);
	            else
	                error(errorMessage);
	        },
	        complete: function () {
	            if (sender) {
	                $(sender).text(prevText);
	                $(sender).removeAttr("disabled");
	            }

	            _locker.Unlock();

	            complete();
	        }
	    });

	}

	this.DataGrid = function (config) {
	    /// <signature>
	    ///   <summary>Build a datagrid inside the container.</summary>
	    ///   &#10;
	    ///   &#10;Config Keys:    	    
	    ///   &#10;1. [string key]: The column id that will be the key of each row.
	    ///   &#10;2. [Array buttons]: A array of buttons that will be placed at datagrid footer.
	    ///   &#10;3. [Function onselect]: A function to be called when a row is selected.
	    ///   &#10;4. [Function ondeselect]: A function to be called when a row is deselected.
	    ///   <param name="config" type="PlainObject">A set of key/value pairs that configure the grid.</param>    
	    /// </signature>

	    if (!container)
	        throw "The \"container\" is required.";

	    if (!config)
	        throw "The \"config\" is required.";

	    var _locker = new LockerHelper({
	        container: locker,
	        text: text
	    });	    

	    $.ajax({
	        url: url,
	        data: data,
	        traditional: traditional,
	        type: "post",
	        dataType: "json",
	        async: async,
	        beforeSend: function () {
	            if (sender) {
	                $(sender).text(text);
	                $(sender).attr("disabled", "disabled");
	            }

	            _locker.Lock();
	            
	            beforeSend();
	        },
	        success: function (data) {	            

	            var input = new InputHelper();

	            var containerClass = config.containerClass;
	            var table = document.createElement("table");
	            var theader = document.createElement("thead");
	            var trHeader = document.createElement("tr");
	            var keys = [];
	            var header = document.createElement("header");
	            var body = document.createElement("article");
	            var footer = document.createElement("footer");
	            body.appendChild(table);

	            if (containerClass && !$(container).hasClass(containerClass))
	                $(container).addClass(containerClass);

	            if (config.size && !isNaN(config.size))
	                $(body).attr("style", "max-height: " + config.size + "px; overflow-y: auto;");

	            $(header).addClass("datagridHeader");
	            $(footer).addClass("datagridFooter");

	            $(header).text(config.title || "XQuery - Datagrid");

	            if (data.columns) {
	                $.each(data.columns, function (iColumn, column) {
	                    var thHeader = document.createElement("th");
	                    thHeader.innerHTML = column.caption;
	                    trHeader.appendChild(thHeader);
	                });
	            }

	            theader.appendChild(trHeader);
	            table.appendChild(theader);

	            var tbody = document.createElement("tbody");

	            if (data.models) {
	                $.each(data.models, function (index, item) {
	                    var trBody = document.createElement("tr");
	                    trBody.setAttribute("data-xquery-datagrid-key", item[config.key || "KeyHandler"]);	                    

	                    $.each(data.columns, function (iColumn, column) {
	                        var ids = column.id.split(".");
	                        var subItem = item;

	                        $.each(ids, function (index, element) {
	                            subItem = subItem[element];
	                        });

	                        var tdBody = document.createElement("td");
	                        var span = document.createElement("span");
	                        var value = column.content || subItem;
	                        
	                        var key = config.key || "KeyHandler";
	                        var bypass = {}
	                        bypass[key] = {}

	                        if (config.onbeforebindcolumn && (config.onbeforebindcolumn instanceof Function))
	                            config.onbeforebindcolumn(key, item, column, value, bypass);

	                        if (bypass[key].value != undefined)
	                            value = bypass[key].value;

	                        if (column.dateformat) {
	                            if (value)
	                                value = new Date(parseInt(value.substr(6)));

	                            var format = column.dateformat.toString().toLowerCase().replace("yy", "y").replace("yy", "y");
	                            value = $.datepicker.formatDate(format, value);
	                        }

	                        if (column.decimals && !isNaN(value)) {
	                            value = Number(value).toFixed(column.decimals);
	                            value = value.replace(".", column.decimalSeparator || ".");
	                        }

	                        if (column.prefix)
	                            value = column.prefix + value;

	                        if (column.sufix)
	                            value = value + column.sufix;
	                        

	                        if (bypass[key].style)
	                            span.setAttribute("style", bypass[key].style);

	                        if (bypass[key].className)
	                            span.setAttribute("class", bypass[key].className);

	                        bypass = {}
	                        bypass[key] = {}
	                        if (config.onafterbindcolumn && (config.onafterbindcolumn instanceof Function))
	                            config.onafterbindcolumn(key, item, column, value, bypass);

	                        if (bypass[key].value)
	                            value = bypass[key].value;

	                        if (!isNaN(value))
	                            span.setAttribute("style", "text-align: right;");

	                        if (bypass[key].style)
	                            span.setAttribute("style", bypass[key].style);

	                        if (bypass[key].className)
	                            span.setAttribute("class", bypass[key].className);

	                        span.innerHTML = value;
	                        tdBody.appendChild(span);
	                        trBody.appendChild(tdBody);
	                    });

	                    trBody.onclick = function () {
	                        if (!input.IsPressed(input.INPUTS.KEY_LEFT_CONTROL) || !config.multiple)
	                            $(".selected").removeClass("selected");

	                        if ($(this).hasClass("selected")) {
	                            $(this).removeClass("selected");

	                            keys = [];
	                            $.each(tbody.getElementsByClassName("selected"), function (index, item) {
	                                keys.push($(item).attr("data-xquery-datagrid-key"));
	                            });

	                            if (config.ondeselect && (config.ondeselect instanceof Function))
	                                config.ondeselect(this, item, keys, item[config.key || "KeyHandler"]);
	                        }
	                        else {
	                            $(this).addClass("selected");

	                            keys = [];
	                            $.each(tbody.getElementsByClassName("selected"), function (index, item) {
	                                keys.push($(item).attr("data-xquery-datagrid-key"));
	                            });

	                            if (config.onselect && (config.onselect instanceof Function))
	                                config.onselect(this, item, keys, item[config.key || "KeyHandler"]);
	                        }
	                    }

	                    tbody.appendChild(trBody);
	                });
	                table.appendChild(tbody);

	                var tfooter = document.createElement("tfoot");
	                var trFooter = document.createElement("tr");
	                var tdFooter = document.createElement("td");
	                tdFooter.setAttribute("colspan", data.columns.length);

	                if (config.buttons) {
	                    $.each(config.buttons, function (iButton, button) {
	                        var buttonFooter = document.createElement("button");

	                        if (button.className)
	                            buttonFooter.className = button.className;

	                        buttonFooter.id = button.id;
	                        buttonFooter.innerText = button.text;
	                        buttonFooter.onclick = function () {
	                            button.func(this, keys, data.models);
	                        }

	                        if (!config.buttonsTo) {
	                            $(footer).children("#" + buttonFooter.id).remove();
	                            $(footer).append(buttonFooter);	                            
	                        }
	                        else {
	                            $(config.buttonsTo).children("#" + buttonFooter.id).remove();
	                            $(config.buttonsTo).append($(buttonFooter));
	                        }
	                    });
	                }

	                trFooter.appendChild(tdFooter);

	                tfooter.appendChild(trFooter);
	                table.appendChild(tfooter);
	            }

	            $(container).empty();
	            $(container).append(header);
	            $(container).append(body);
	            $(container).append(footer);

	            success(data);
	        },
	        error: function (jqXHR, textStatus, errorThrown) {

	            var str = jqXHR.responseText;
	            var n = str.search("<title>") + 1;
	            var e = str.search("</title>");
	            str = str.substring(n, e);

	            var errorMessage = str.substring(str.search(">") + 1, e);
	            if (!errorMessage)
	                error(errorThrown);
	            else
	                error(errorMessage);
	        },
	        complete: function () {
	            if (sender) {
	                $(sender).text(prevText);
	                $(sender).removeAttr("disabled");
	            }

	            _locker.Unlock();

	            complete();
	        }
	    });

	}
}

AjaxHelper.LoadString = function(url, data) {
    /// <signature>
    ///   <summary>Exec a synchronous GET and return the result as string.</summary>    
    ///   <param name="url" type="string">The url to load the string.</param>    
    ///   <param name="data" type="PlainObject">The data to send with the request.</param>    
    ///   <returns type="String" />
    /// </signature> 	           

    var result = "#Not Found#";

    $.ajax({
        url: url,
        data: data,
        type: "get",
        async: false,
        success: function (data) {
            result = data.toString();
        },
        error: function (jqXHR, textStatus, errorThrown) {

            var str = jqXHR.responseText;
            var n = str.search("<title>") + 1;
            var e = str.search("</title>");
            str = str.substring(n, e);

            var errorMessage = str.substring(str.search(">") + 1, e);
            if (!errorMessage)
                result = errorThrown;
            else
                result = errorMessage;
        }
    });

    return result;
}

AjaxHelper.ModelBind = function (sender, model, mapper, entity) {
    /// <signature>
    ///   <summary>Bind the values of the model to a form.</summary>    
    ///   <param name="sender" type="JQuery">The form itself or some child element of the form.</param>    
    ///   <param name="model" type="PlainObject">The json data to bind to the form.</param>
    ///   <param name="mapper" type="PlainObject">A key/value set to map the properties to different fields.</param>
    ///   <param name="entity" type="string">The class name of the model to be binded. Used when the model itself is a view model.</param>        
    /// </signature> 	

    var form = $(sender)[0].tagName.toLowerCase() == "form" ? $(sender) : $(sender).parents('form');

    if (!model)
        throw "The \"model\" is required.";

    if (!form)
        throw "The \"form\" is required.";

    if (entity)
        model = model[entity];

    if (!model)
        throw "The \"entity\" are incorrect or not exists to this model.";

    $.each(model, function (index, item) {

        var
            field = form.find('*[name="' + (entity ? entity + "." + index : index) + '"]'),
            type = "", tag = "";

        if (mapper && mapper[index])
            field = form.find('*[name="' + (entity ? entity + "." + mapper[index] : mapper[index]) + '"]');

        if (field.length > 0) {

            tag = field[0].tagName.toLowerCase();

            if (tag == "select" || tag == "textarea")
                field.val(item);
            else if (tag == "input") {

                type = $(field[0]).attr("type");

                if (type == "checkbox") {
                    if (item)
                        field.attr("checked", "checked");
                }
                else if (type == "radio")
                    field.filter('[value="' + item + '"]').attr("checked", "checked");
                else
                    field.val(item);
            }
        }
    });
}

AjaxHelper.QuickPost = function(url, data){
    /// <signature>
    ///   <summary>Exec a synchronous POST and return the result.</summary>    
    ///   <param name="url" type="string">The url to load the string.</param>    
    ///   <param name="data" type="PlainObject">The data to send with the request.</param>    
    ///   <returns type="PlainObject" />
    /// </signature> 	

    var result = {}

    $.ajax({
        url: url,
        data: data,
        type: "post",
        async: false,
        success: function (data) {
            result = data;
        },
        error: function (jqXHR, textStatus, errorThrown) {

            var str = jqXHR.responseText;
            var n = str.search("<title>") + 1;
            var e = str.search("</title>");
            str = str.substring(n, e);

            var errorMessage = str.substring(str.search(">") + 1, e);
            if (!errorMessage)
                result = errorThrown;
            else
                result = errorMessage;
        }
    });

    return result;
}

AjaxHelper.QuickGet = function (url, data) {
    /// <signature>
    ///   <summary>Exec a synchronous POST and return the result.</summary>    
    ///   <param name="url" type="string">The url to load the string.</param>    
    ///   <param name="data" type="PlainObject">The data to send with the request.</param>    
    ///   <returns type="PlainObject" />
    /// </signature> 	

    var result = {}

    $.ajax({
        url: url,
        data: data,
        type: "get",
        async: false,
        success: function (data) {
            result = data;
        },
        error: function (jqXHR, textStatus, errorThrown) {

            var str = jqXHR.responseText;
            var n = str.search("<title>") + 1;
            var e = str.search("</title>");
            str = str.substring(n, e);

            var errorMessage = str.substring(str.search(">") + 1, e);
            if (!errorMessage)
                result = errorThrown;
            else
                result = errorMessage;
        }
    });

    return result;
}