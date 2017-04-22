$.widget("ui.ajaxdropdown",{
    version: "1.0.0",
    _create: function() {
        var that = this;
        this.element.on("change", function () {
            var url = $(this).val();
            $("#" + that.options.target).load(url);
        });
    }
});