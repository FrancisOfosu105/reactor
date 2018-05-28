import FileInput from "./modules/file-input";
import DateTime from "./modules/date-time";
import Post from "./modules/post";
import Modal from "./modules/modal";
import Follow from "./modules/follow";
import AjaxLoader from "./modules/ajax-loader";
import Header from "./modules/header";

new Header();
new AjaxLoader();
new FileInput();
new DateTime();
new Post();
new Modal();
new Follow();

/*
* Lazy loading Hack
* Will improve it later
* */
const index = window.location.pathname.lastIndexOf('/');
const url = window.location.pathname.substr(index + 1);

switch (url) {
    case "chat":
        import("./modules/chat").then((module: any) => {
            new module.default();
        });
        break;
    case "notifications":
        import("./modules/notification").then((module: any) => {
            new module.default();
        });
        break;
} 
 