"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var $ = require("jquery");
var timeago_js_1 = require("timeago.js");
var addAntiForgeryToken = function (data) {
    if (!data) {
        data = {};
    }
    var tokenInput = $("input[name=__RequestVerificationToken]");
    if (tokenInput.length) {
        data.__RequestVerificationToken = tokenInput.val();
    }
    return data;
};
var addTimeago = function () {
    var $time = $("time.timeago");
    timeago_js_1.default().render($time);
};
exports.default = { addAntiForgeryToken: addAntiForgeryToken, addTimeago: addTimeago };
