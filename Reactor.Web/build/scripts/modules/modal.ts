export default class Modal {
    private $imgOverlayContainer = $(".img-overlay");
    private $imgOverlayImg = $(".img-overlay__photo");
    private $closeOverBtn = $(".img-overlay__close");
    private $body =$("body");
    private $document= $(document);
    constructor(){
        this.events();
    }
    private events(){
        this.$body.on("click","img[data-overlay]", this.openModal.bind(this));
        this.$closeOverBtn.on("click",this.closeModal.bind(this));
        this.$document.on("keyup", this.keyUpHandler.bind(this));
        this.$document.on("scroll", this.closeModal.bind(this));
        this.$imgOverlayContainer.on("click", this.closeModal.bind(this));

    }
    
    private keyUpHandler(e:KeyboardEvent){
        if (e.keyCode ===27)
            this.closeModal();
    }

    private openModal(e:Event) {
        let $imgElem = $(e.target);
        let src = $imgElem.attr("src");
        this.$imgOverlayImg.attr("src", src);
        this.$imgOverlayContainer.addClass("img-overlay--is-visible");
        
    }
    
    private closeModal(){
        this.$imgOverlayImg.attr("src", "");
        this.$imgOverlayContainer.removeClass("img-overlay--is-visible");
    }
}