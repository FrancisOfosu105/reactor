import * as $ from 'jquery';

export default class AjaxLoader {
    constructor(){
        this.configure();
    }
    
    private configure =()=>{
        $(document).ajaxStart(()=>{
            $(".loader").addClass("loader--show");
            
        }).ajaxStop(()=>{
            $(".loader").removeClass("loader--show");
        });
    }
}   