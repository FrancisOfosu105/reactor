"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var timeago_js_1 = require("timeago.js");
var $ = require("jquery");
var DateTime = (function () {
    function DateTime() {
        this.$time = $("time.timeago");
        this.addTimeago();
    }
    DateTime.prototype.addTimeago = function () {
        timeago_js_1.default().render(this.$time);
    };
    return DateTime;
}());
exports.default = DateTime;
