import timeago from "timeago.js";
import * as $ from "jquery";
export default class DateTime{
   private $time = $("time.timeago");

    constructor() {
        this.addTimeago();
    }
    
   private addTimeago(){
        timeago().render(this.$time);
    }
}