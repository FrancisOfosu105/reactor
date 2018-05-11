import * as $ from "jquery";

export default class FileInput {
    private $fileInput = $(".js-inputfile");

    constructor() {
        this.configure();
    }

    private configure() {
        const that = this;

        this.$fileInput.each(function() {

            const currentFileInputElem: any = this;

            const fileInput = $(currentFileInputElem);
            const label = fileInput.next();
            const labelValue = label.html();

            fileInput.on("change",
                (e: any) => {
                    let fileName = "";


                    if (!(currentFileInputElem.files && currentFileInputElem.files.length > 1)) {
                        if (e.target.value) {
                            fileName = that.getFileName(e.target.value);
                        }
                    } else {

                        fileName = (currentFileInputElem.getAttribute("data-multiple-caption") || "").replace("{count}",
                            currentFileInputElem.files.length);
                    }

                    if (fileName) {
                        label.find("span").html(fileName);
                    } else {
                        label.html(labelValue);
                    }

                });

        });
    }

    private getFileName(value: any) {
        return value.split("\\").pop();
    }
}