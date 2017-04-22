$.widget("ui.ajaxdropdown",{
    version: "1.0.0",
    _create: function() {
        var that = this;
        this.element.on("change", function () {
            var url = $(this).val();
            $("#" + that.options.target).loadWithImages(url);
        });
    }
});

$.fn.loadWithImages = function(url) {
    var $that = this;
    $.get(url).done(function (data) {
        var totalImg = 0;
        var loadedImg = 0;

        function onImageLoad() {
            loadedImg++;
            if (totalImg == loadedImg) {
                $that.html(data);
            }
        }

        $(data).find("img").each(function() {
            totalImg++;
            var img = new Image();
            img.onload = onImageLoad;
            img.onerror = onImageLoad;
            img.src = $(this).attr("src");
        });
    });
};