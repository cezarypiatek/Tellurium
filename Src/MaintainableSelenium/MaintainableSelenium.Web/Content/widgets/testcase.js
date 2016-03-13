$.widget("ui.testcase", {
    version: "1.0.0",
    _create: function () {
        var drawing = false;
        var startPoint;
        var $label = this.element.find(".label");
        var that = this;
        var currSquare;
        var locked = false;

        function getNumber(percentageText) {
            return parseFloat(percentageText.replace("%", ""));
        }
        this.element.on("click", "[data-testcase-element='save']", function () {
            if (locked) {
                return;
            }
            var maxWidth = that.element.width();
            var maxHeight = that.element.height();
            var dataToPost = [];
            that.element.find(".blind").each(function() {
                var $this = $(this);
                dataToPost.push({
                    left: getNumber($this.css("left"))/maxWidth*100.0,
                    top: getNumber($this.css("top"))/maxHeight*100,
                    width: getNumber($this.css("width")) / maxWidth * 100.0,
                    height: getNumber($this.css("height")) / maxHeight * 100
                });
            });
            locked = true;
            $.postJSON(that.options.actionsave, {
                TestCaseId: that.options.id,
                BlindRegions: dataToPost
            }).done(function () {
                locked = false;
            });
        });
        this.element.on("keyup", ".blind", function (e) {
            if (e.keyCode == 46) {
                that.element.find(".blind.active").remove();
            }
        });

        this.element.on("focus", ".blind", function (e) {
            $(this).addClass("active");
        });

        this.element.on("blur", ".blind.active", function (e) {
            $(this).removeClass("active");
        });

        this.element.on("mousedown", ".blind", function(e) {
            e.stopPropagation();
        });
        var index = 1;

        this.options.regions.forEach(function(data) {
            var square = $("<div></div>", { "class": "blind", tabindex: index++ });
            square.css({
                left: data.Left + "%",
                top: data.Top + "%",
                width: data.Width + "%",
                height: data.Height+ "%"
            });
            that.element.append(square);
        });

        this.element.on("mousedown", function (e) {
            drawing = true;
            startPoint = getOffset(e);
            currSquare = $("<div></div>",{"class":"blind", tabindex:index++});
            that.element.append(currSquare);
        });
        this.element.on("mouseup", function() {
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
            var parentOffset = that.element.offset();
            return {
                x: e.pageX - parentOffset.left,
                y: e.pageY - parentOffset.top
            };
        }

        this.element.on("mousemove", function(e) {
            var offset = getOffset(e);

            $label.text("X:" + offset.x + " Y:" + offset.y);

            if (drawing) {
                var maxWidth = that.element.width();
                var maxHeight = that.element.height();
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