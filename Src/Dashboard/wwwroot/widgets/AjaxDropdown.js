$.widget("ui.ajaxdropdown",{
    version: "1.0.0",
    _create: function() {
        var that = this;
        this.element.on("change", function () {
            var url = $(this).val();
            $(that.options.target).loadWithImages(url);
        });
    }
});

$.fn.loadWithImages = function(url) {
    var $that = this;
    var dfd = jQuery.Deferred();
    $.get(url).done(function (data) {
        var totalImg = 0;
        var loadedImg = 0;

        function completeLoading() {
            $that.html(data);
            dfd.resolve();
        }

        function onImageLoad() {
            loadedImg++;
            if (totalImg == loadedImg) {
                completeLoading();
            }
        }


        var $images = $(data).find("img");
        $images.each(function() {
            totalImg++;
            var img = new Image();
            img.onload = onImageLoad;
            img.onerror = onImageLoad;
            img.src = $(this).attr("src");
        });

        if ($images.length == 0) {
            completeLoading();
        }
    });
    return dfd.promise();
};
$(function() {
    $("body").on("click", "[role='tab']", function (e) {
        var src = $(this).data("src");
        if (src) {
            var containerSelector = $(this).attr("href");
            var $container = $(containerSelector);
            if ($container.data("loaded") != true) {
                $container.loadWithImages(src).done(function() {
                    $container.data("loaded", true);
                });
            }
        }
    });
});
