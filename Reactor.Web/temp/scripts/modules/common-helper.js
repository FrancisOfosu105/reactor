import $ from 'jquery';
import timeago from 'timeago.js';

let addAntiForgeryToken = (data) => {
    if (!data) {
        data = {};
    }

    let tokenInput = $('input[name=__RequestVerificationToken]');

    if (tokenInput.length) {
        data.__RequestVerificationToken = tokenInput.val();

    }

    return data;
};


let addTimeago = () => {

    let $time = $('time.timeago');
    timeago().render($time);
};


export default {addAntiForgeryToken, addTimeago};




