"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var $ = require("jquery");
var FileInput = (function () {
    function FileInput() {
        this.$fileInput = $(".js-inputfile");
        this.configure();
    }
    FileInput.prototype.configure = function () {
        var that = this;
        this.$fileInput.each(function () {
            var currentFileInputElem = this;
            var fileInput = $(currentFileInputElem);
            var label = fileInput.next();
            var labelValue = label.html();
            fileInput.on("change", function (e) {
                var fileName = "";
                if (!(currentFileInputElem.files && currentFileInputElem.files.length > 1)) {
                    if (e.target.value) {
                        fileName = that.getFileName(e.target.value);
                    }
                }
                else {
                    fileName = (currentFileInputElem.getAttribute("data-multiple-caption") || "").replace("{count}", currentFileInputElem.files.length);
                }
                if (fileName) {
                    label.find("span").html(fileName);
                }
                else {
                    label.html(labelValue);
                }
            });
        });
    };
    FileInput.prototype.getFileName = function (value) {
        return value.split("\\").pop();
    };
    return FileInput;
}());
exports.default = FileInput;
