import timeago from "timeago.js";
import * as $ from "jquery";
export default class DateTime{
    $time = $("time.timeago");

    constructor() {
        this.addTimeago();
    }
    
    addTimeago(){
        timeago().render(this.$time);
    }
}