import * as $ from "jquery";

export default class FileInput {
    $fileInput = $(".js-inputfile");

    constructor() {
        this.configure();
    }

    configure() {

        this.$fileInput.on("change",
            (e: any) => {
                const $this: any = $(e.target);
                const label = $this.next();
                const labelValue = label.html();

                let fileName = "";

                if (!($this.files && $this.files.length > 1)) {
                    if (e.target.value) {
                        fileName = FileInput.getFileName(e.target.value);
                    }
                } else {
                    
                    fileName = ($this.attr("data-multiple-caption") || "").replace("{count}",
                        $this.files.length);
                }

                if (fileName) {
                    label.find("span").html(fileName);
                } else {
                    label.html(labelValue);
                }


            });
    }

    static getFileName(value: any) {
        return value.split("\\").pop();
    }


}