import * as $ from "jquery";
import timeago from "timeago.js";

let addAntiForgeryToken = (data: any) => {
    if (!data) {
        data = {};
    }

    const tokenInput = $("input[name=__RequestVerificationToken]");


    if (tokenInput.length) {
        data.__RequestVerificationToken = tokenInput.val();

    }

    return data;
};


let updateTimeago = () => {

    const $time = $("time.timeago");
    timeago().render($time);
};


export default { addAntiForgeryToken, updateTimeago };




