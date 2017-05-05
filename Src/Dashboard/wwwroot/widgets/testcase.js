
function ChangeTracker(getGroupNameFunc) {
    var changeInfo = { };
    var newElements = [];

    var getGroupName = getGroupNameFunc || function() { return "main"; };

    this.elementChanged = function(element, operation) {
        if (operation == "add") {
            newElements.push(element);
        } else {
            
            var isNewElement = newElements.some(function (el) {
                return el == element;
            });

            if (isNewElement) {
                if (operation == "delete") {
                    newElements = newElements.filter(function(el) {
                        return el != element;
                    });
                }
            } else {
                var elementGroupName = getGroupName(element);
                changeInfo[elementGroupName] = true;
            }
        }
    };

    this.wasChange = function(group) {
        return changeInfo[group] || newElements.some(function(el) {
            return getGroupName(el) == group;
        });
    };

    this.reset = function() {
        changeInfo = {};
        newElements = [];
    };
}

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
        var changeTracker = new ChangeTracker(function (element) {
            var $element = $(element);
            if ($element.hasClass("local")) {
                return "local";
            } else if ($element.hasClass("category")) {
                return "category";
            }
            if ($element.hasClass("global")) {
                return "global";
            }
            throw Error("Unknow group");
        });


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
            var categorySpots = [];
            var globalSpots = [];
            var screenshotDimensions = that._getScreenshotDimensions();
            $board.find(".blind").each(function () {
                var $this = $(this);
                var spot ={
                    left: getNumber($this.css("left")) / maxWidth * screenshotDimensions.width,
                    top: getNumber($this.css("top")) / maxHeight * screenshotDimensions.height,
                    width: getNumber($this.css("width")) / maxWidth * screenshotDimensions.width,
                    height: getNumber($this.css("height")) / maxHeight * screenshotDimensions.height
                };
                if ($this.is(".local")) {
                    localSpots.push(spot);
                }else if ($this.is(".category")) {
                    categorySpots.push(spot);
                }else {
                    globalSpots.push(spot);
                }
            });
            locked = true;


            var saveInputModel = {};

            if (changeTracker.wasChange("local")) {
                saveInputModel.Local = {
                    BrowserPatternId: that.options.id,
                    TestCaseId: that.options.caseId,
                    LocalBlindRegions: localSpots
                };
            }

            if (changeTracker.wasChange("category")) {
                saveInputModel.Category = {
                    BlindRegions: categorySpots,
                    BrowserName: that.options.browser,
                    TestCaseId: that.options.caseId
                };
            }

            if (changeTracker.wasChange("global")) {
                saveInputModel.Global = {
                    BlindRegions: globalSpots,
                    BrowserName: that.options.browser,
                    TestCaseId: that.options.caseId
                };
            }

            $.postJSON(that.options.actionSave, saveInputModel).done(function () {
                    changeTracker.reset();
                    locked = false;
                });
        });
        $board.on("keyup", ".blind", function (e) {
            if (e.keyCode == 46) {
                var $elementToRemove = $board.find(".blind.active");
                changeTracker.elementChanged($elementToRemove.get(0), "delete");
                $elementToRemove.remove();
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

        function addSquareToBoard(data, type) {
            var screenshotDimensions = that._getScreenshotDimensions();
            var square = $("<div></div>", { "class": "blind "+type, tabindex: index++ });
            $board.append(square);
            square.css({
                left: data.Left / screenshotDimensions.width * 100.0 + "%",
                top: data.Top / screenshotDimensions.height * 100 + "%",
                width: data.Width / screenshotDimensions.width * 100.0 + "%",
                height: data.Height / screenshotDimensions.height * 100.0 + "%"
            });
        }

        this.element.find(".screenshot").on("load", function() {
            that.options.localRegions.forEach(function (data) {
                addSquareToBoard(data, "local");
            });

            that.options.categoryRegions.forEach(function (data) {
                addSquareToBoard(data, "category");
            });

            that.options.globalRegions.forEach(function (data) {
                addSquareToBoard(data, "global");
            });
        });

        var moving = false;
        var $currentMovingSquare = null;
        var moveStartOffset = null;
        var currentMovingSquareOriginalOffset = null;
        $board.on("mousedown", ".blind", function(e) {
            moving = true;
            moveStartOffset = getOffset(e);
            $currentMovingSquare = $(this);
            changeTracker.elementChanged($currentMovingSquare.get(0), "move");
            var offset = $currentMovingSquare.offset();
            currentMovingSquareOriginalOffset = getOffset({ pageX: offset.left, pageY: offset.top});
        });
        
        $board.on("mousedown", function (e) {
            drawing = true;
            startPoint = getOffset(e);
            currSquare = $("<div></div>",{"class":"blind "+scope, tabindex:index++});
            $board.append(currSquare);
            changeTracker.elementChanged(currSquare.get(0), "add");
        });

        $("body").on("mouseup", function () {
            if (drawing) {
                drawing = false;
                if (currSquare.width() < 5 || currSquare.height() < 5) {
                    changeTracker.elementChanged(currSquare.get(0), "delete");
                    currSquare.remove();
                }
                currSquare.focus();
                currSquare = null;
            }

            if (moving) {
                moving = false;
                $currentMovingSquare = null;
            }
        });

        function getOffset(e) {
            var parentOffset = $board.offset();
            return {
                x: e.pageX - parentOffset.left,
                y: e.pageY - parentOffset.top
            };
        }

        var lastOffset = null;

        $board.on("mouseleave", function(e) {
            var offset = getOffset(e);
            var maxWidth = $board.width();
            var maxHeight = $board.height();

            if (offset.x < 0) {
                offset.x = 0;
            } else if (offset.x > maxWidth) {
                offset.x = maxWidth;
            }

            if (offset.y < 0) {
                offset.y = 0;
            } else if (offset.y > maxHeight) {
                offset.y = maxHeight;
            }

            $label.text("X:" + offset.x + " Y:" + offset.y);

            if (drawing) {
                currSquare.css({
                    left: Math.min(startPoint.x, offset.x) / maxWidth * 100 + "%",
                    top: Math.min(startPoint.y, offset.y) / maxHeight * 100 + "%",
                    width: Math.abs(offset.x - startPoint.x) / maxWidth * 100 + "%",
                    height: Math.abs(offset.y - startPoint.y) / maxHeight * 100 + "%"
                });
            }
        });

        $board.on("mousemove", function (e) {
            var offset = getOffset(e);

            $label.text("X:" + offset.x + " Y:" + offset.y);
            var maxWidth = $board.width();
            var maxHeight = $board.height();

            if (drawing) {
                currSquare.css({
                    left: Math.min(startPoint.x, offset.x) / maxWidth *100 + "%",
                    top: Math.min(startPoint.y, offset.y) / maxHeight * 100 + "%",
                    width: Math.abs(offset.x - startPoint.x) / maxWidth * 100 + "%",
                    height: Math.abs(offset.y - startPoint.y) / maxHeight * 100 + "%"
                });
            }

            if (moving) {

                var xOff = offset.x - moveStartOffset.x;
                var yOff = offset.y - moveStartOffset.y;
                var newX = currentMovingSquareOriginalOffset.x + xOff;
                var newY = currentMovingSquareOriginalOffset.y + yOff;
                var squareWidth = $currentMovingSquare.outerWidth();
                var squareHeight = $currentMovingSquare.outerHeight();

                var newRight = newX + squareWidth;
                var newBottom = newY + squareHeight;
                if (newX < 0) {
                    newX = 0;
                } else if (newRight > maxWidth) {
                    newX = maxWidth - squareWidth;
                }

                if (newY < 0) {
                    newY = 0;
                } else if (newBottom > maxHeight) {
                    newY = maxHeight - squareHeight;
                }
                
                $currentMovingSquare.css({
                    left: newX / maxWidth * 100 + "%",
                    top: newY / maxHeight * 100 + "%"
                });    
                
            }
            lastOffset = offset;
        });
    },
    _getScreenshotDimensions: function() {
        var screenshot = this.element.find(".screenshot").get(0);
        return {
            width: screenshot.naturalWidth,
            height: screenshot.naturalHeight
        };
    }
});