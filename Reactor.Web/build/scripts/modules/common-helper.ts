import * as $ from "jquery";
import tago from "timeago.js";

const addAntiForgeryToken = (data: any) => {
    if (!data) {
        data = {}; 
    }

    const tokenInput = $("input[name=__RequestVerificationToken]");


    if (tokenInput.length) {
        data.__RequestVerificationToken = tokenInput.val();

    }

    return data;
};


const timeago = {

    reInitialize: () => {
        const $time = $("time.timeago");
        tago().render($time);
    }

};


export default {addAntiForgeryToken, timeago};




