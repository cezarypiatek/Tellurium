$.widget("ui.remoteexpander", {
    version: "1.0.0",
    _create: function () {
        var that = this;
        var isLoaded = false;
        var $target = $("#" + this.options.targetid);

        this.element.on("click", function() {
            if (isLoaded == false) {
                var $loader = $("<span></span>", { "class": "glyphicon glyphicon-refresh glyphicon-refresh-animate" });
                that.element.append($loader);
                $target.load(that.options.source, function () {
                    var $images = $target.find("img");
                    var imagesToLoad = $images.length;
                    function finishLoading() {
                            isLoaded = true;
                            $target.removeClass("collapse");
                            $loader.remove();
                    };

                    function tryToFinishLoad() {
                        imagesToLoad--;
                        if (imagesToLoad == 0) {
                            finishLoading();
                        }
                    }

                    if (imagesToLoad == 0) {
                        finishLoading();
                    } else {
                        $images.on("load", tryToFinishLoad).on("error", tryToFinishLoad);
                    }
                });
            } else {
                $target.toggleClass("collapse");
            }
        });
    }
});