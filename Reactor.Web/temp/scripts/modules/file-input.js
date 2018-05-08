import $ from "jquery";

export default class FileInput {
    constructor() {
        this.fileInput = $('.js-inputfile');
        this.configure();
    }

    configure() {
       this.fileInput.each(function () {

            let fileInput = $(this),
                label = fileInput.next(),
                labelValue = label.html();

            fileInput.on('change', function (e) {
                let fileName = '';
                
                 
                
                if (!(this.files && this.files.length > 1)) {
                    if (e.target.value) {
                        fileName = FileInput.getFileName(e.target.value);
                    }
                }
                else {
                    
                    fileName = (this.getAttribute('data-multiple-caption') || '').replace('{count}', this.files.length);
                }

                if (fileName) {
                    label.find('span').html(fileName);
                } else {
                    label.html(labelValue);
                }

            });

        })
    }

    static getFileName(value) {
        return value.split('\\').pop();
    }

  
        
    
}