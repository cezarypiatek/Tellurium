var baseReplaceWith = $.fn.replaceWith;
$.fn.replaceWith = function () {
    var element;
    if (typeof arguments[0] == "string") {
        element = arguments[0] = $(arguments[0]);
    }
    var res = baseReplaceWith.apply(this, arguments);
    if (element != null) {
        $.ui.modernUnobtrusive.parse(element);
    }
    return res;
};

var baseReplaceHtml = $.fn.html;
$.fn.html = function () {
    var element;
    if (typeof arguments[0] == "string") {
        element = arguments[0] = $(arguments[0]);
    }
    var res = baseReplaceHtml.apply(this, arguments);
    if (element != null) {
        $.ui.modernUnobtrusive.parse(element);
    }
    return res;
};


$.postJSON = function (url, jsonObj, options) {
    var defaultOptions = {
        contentType: "application/json; charset=utf-8",
        type: "POST",
        url: url,
        data: JSON.stringify(jsonObj)
    };
    $.extend(defaultOptions, options);
    return $.ajax(defaultOptions);
};