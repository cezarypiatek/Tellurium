(function ($) {
    "use strict";

    /**
     * Extract widget options from DOM element
     * @param {string} widgetName
     * @param {HTMLElement} element
     *
     * @returns {object}
     */
    function getWidgetOptions(widgetName, element) {
        var options = {};
        var optionPrefix = ("data-ui-" + widgetName + "-").toLowerCase();
        Array.prototype.forEach.call(element.attributes, function(attr) {
            if (attr.specified == false || attr.name.indexOf(optionPrefix) !== 0) {
                return;
            }

            var optionName = $.camelCase(attr.name.substr(optionPrefix.length));
            var optionDataKey = attr.name.substr(5);
            options[optionName] = $(element).data(optionDataKey);
        });
        return options;
    }

    $.ui.modernUnobtrusive = {
        /**
         * Wires up jQuery UI unobtrusively based on data-* attributes for the passed element and its children.
         * @param {jQuery|HTMLElement} context A DOM Element, Document, or jQuery to use as context
         */
        parse: function (context) {
            $("[data-ui-fn]", context).each(function () {
                $.ui.modernUnobtrusive.parseElement(this);
            });
            if ($(context).is("[data-ui-fn]")) {
                $.ui.modernUnobtrusive.parseElement($(context));
            }
        },

        /**
         * Wires up jQuery UI unobtrusively on a single element.
         * @param {jQuery|HTMLElement} el A DOM Element, selector or jQuery collection.
         */
        parseElement: function (el) {
            var $el = $(el);
            var element = $el[0];

            var requestedWidgetsNames = $el.attr("data-ui-fn").split(" ");
            requestedWidgetsNames.forEach(function (widgetName) {
                var uiFn = $el[widgetName];
                if (uiFn) {
                    if ($el.data(widgetName) !== undefined) {
                        uiFn.call($el, 'destroy');
                    }
                    var options = getWidgetOptions(widgetName, element);
                    uiFn.call($el, options);
                }
            });
        }
    };

    $(function () {
        $.ui.modernUnobtrusive.parse(window.document);
    });

}(window.jQuery));