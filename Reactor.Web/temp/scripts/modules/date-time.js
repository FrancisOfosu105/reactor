import timeago from 'timeago.js';
import $ from 'jquery';
export default class DateTime{
    constructor(){
        this.time =$('time.timeago');
        this.addTimeago();
    }
    
    addTimeago(){
        timeago().render(this.time);
    }
}