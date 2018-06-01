import tago from "timeago.js";
import "jquery-slimscroll";

const tokenInput = $("input[name=__RequestVerificationToken]");

const antiForgeryToken = {
    add: (data) => {
        if (!data) {
            data = {};
        }


        if (tokenInput.length) {
            data.__RequestVerificationToken = tokenInput.val();

        }

        return data;
    },
    token: () => {
        return tokenInput.val();
    }
};

const timeago = {

    reInitialize: () => {
        const $time = $("time.timeago");
        tago().render($time);
    }

};

const tooltip = {
    init: () => {
        const $allTooltips: any = $('[data-toggle="tooltip"]');

        //Tooltip
        $allTooltips.tooltip();
    },
    reInitialize: () => {
        const $allTooltips: any = $('[data-toggle="tooltip"]');

        $allTooltips.tooltip();
    }
};

export default {antiForgeryToken, timeago, tooltip};




