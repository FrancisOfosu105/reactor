import * as signalR from "@aspnet/signalr";
import * as $ from "jquery";

export default class Notification {
    private chatConnection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build(); 
    constructor() {
        this.setup();
    }

    private setup() {

        this.chatConnection.on("notify", (notifications) => {
            const $oldNotifications = $("li#notifications");
            console.log($oldNotifications);
            if ($oldNotifications.length) {
                $oldNotifications.remove();
                $(".navbar-nav").prepend(notifications);

            }

        });

        this.chatConnection.start().catch(err => console.log(err));
    }
}