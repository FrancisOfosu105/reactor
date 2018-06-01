import timeago from "timeago.js";
export default class DateTime{
   private $time = $("time.timeago");

    constructor() {
        this.addTimeago();
    }
    
   private addTimeago(){
        timeago().render(this.$time);
    }
}