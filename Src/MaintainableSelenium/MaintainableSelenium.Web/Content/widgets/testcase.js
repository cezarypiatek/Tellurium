$.widget("ui.testcase", {
    version: "1.0.0",
    _create: function () {
        var drawing = false;
        var startPoint;
        var $label = this.element.find(".label");
        var that = this;
        var currSquare;
        var locked = false;
        var scope = "local";
        this.element.find("[data-testcase-element='scope']").on("click", ".btn", function() {
            $(this).siblings().removeClass("active");
            $(this).addClass("active");
            scope = $(this).data("testcase-element");
        });

        var $board = this.element.find("[data-testcase-element='board']");
        function getNumber(percentageText) {
            return parseFloat(percentageText.replace("%", ""));
        }
        this.element.on("click", "[data-testcase-element='save']", function (e) {
            e.preventDefault();
            if (locked) {
                return;
            }
            var maxWidth = $board.width();
            var maxHeight = $board.height();
            var localSpots = [];
            var globalSpots = [];
            $board.find(".blind").each(function () {
                var $this = $(this);
                var spot ={
                    left: getNumber($this.css("left"))/maxWidth*100.0,
                    top: getNumber($this.css("top"))/maxHeight*100,
                    width: getNumber($this.css("width")) / maxWidth * 100.0,
                    height: getNumber($this.css("height")) / maxHeight * 100
                }
                if ($this.is(".local")) {
                    localSpots.push(spot);
                } else {
                    globalSpots.push(spot);
                }
                
            });
            locked = true;
            if ($(this).is(".local")) {
                $.postJSON(that.options.actionsave, {
                    TestCaseId: that.options.id,
                    LocalBlindRegions: localSpots
                }).done(function() {
                    locked = false;
                });
            } else {
                $.postJSON(that.options.actionsaveglobal, {
                    BlindRegions: globalSpots,
                    BrowserName: that.options.browser
                }).done(function () {
                    locked = false;
                });
            }
            
        });
        $board.on("keyup", ".blind", function (e) {
            if (e.keyCode == 46) {
                $board.find(".blind.active").remove();
            }
        });

        $board.on("focus", ".blind", function (e) {
            $(this).addClass("active");
        });

        $board.on("blur", ".blind.active", function (e) {
            $(this).removeClass("active");
        });

        $board.on("mousedown", ".blind", function (e) {
            e.stopPropagation();
        });
        var index = 1;

        this.options.regions.forEach(function(data) {
            var square = $("<div></div>", { "class": "blind local", tabindex: index++ });
            $board.append(square);
            square.css({
                left: data.Left + "%",
                top: data.Top + "%",
                width: data.Width + "%",
                height: data.Height + "%"
            });
        });

        this.options.globalregions.forEach(function (data) {
            var square = $("<div></div>", { "class": "blind global", tabindex: index++ });
            $board.append(square);
            square.css({
                left: data.Left + "%",
                top: data.Top + "%",
                width: data.Width + "%",
                height: data.Height + "%"
            });
        });

        $board.on("mousedown", function (e) {
            drawing = true;
            startPoint = getOffset(e);
            currSquare = $("<div></div>",{"class":"blind "+scope, tabindex:index++});
            $board.append(currSquare);
        });
        $board.on("mouseup", function () {
            if (drawing) {
                drawing = false;
                if (currSquare.width() < 5 || currSquare.height() < 5) {
                    currSquare.remove();
                }
                currSquare.focus();
                currSquare = null;
            }
        });

        function getOffset(e) {
            var parentOffset = $board.offset();
            return {
                x: e.pageX - parentOffset.left,
                y: e.pageY - parentOffset.top
            };
        }

        $board.on("mousemove", function (e) {
            var offset = getOffset(e);

            $label.text("X:" + offset.x + " Y:" + offset.y);

            if (drawing) {
                var maxWidth = $board.width();
                var maxHeight = $board.height();
                currSquare.css({
                    left: Math.min(startPoint.x, offset.x) / maxWidth *100 + "%",
                    top: Math.min(startPoint.y, offset.y) / maxHeight * 100 + "%",
                    width: Math.abs(offset.x - startPoint.x) / maxWidth * 100 + "%",
                    height: Math.abs(offset.y - startPoint.y) / maxHeight * 100 + "%"
                });
            }
        });
    }
});